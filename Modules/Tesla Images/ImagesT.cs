using System.Text.RegularExpressions;
using  System.Data.SqlClient;
using System.IO.Compression;

namespace TestWebApi.Modules.Tesla_Images;

public class ImagesT{

    [Theory]
    [InlineData(@"\\mxgdld0nsifsn03\GDL_TE_VOL_1\TE_Customers\Tesla\Image Files\1735623-01-B_TOP\20221213\20221213134028-FE222346AV46.JPG")]
    public bool validateSerialNumberFromImagePathTest(string CompleteimagePath)
    {
        string imageFileName = Path.GetFileName(CompleteimagePath);
        //The serial number starts with FE2 then 5 digits, then A then first leter of orientation(horizontal or vertical) then any digit with 
        string serialNumber = Regex.Match(imageFileName, @"FE2\d{5}A[HV]\d[0-9]+").Value;
        return serialNumber.Length > 8;
        Assert.True(serialNumber.Length > 8);
    }

    [Theory]
    [InlineData(@"\\mxgdld0nsifsn03\GDL_TE_VOL_1\TE_Customers\Tesla\Image Files\1735623-01-B_TOP\20221215\20221215135251-FE222349AH5138.JPG")]
    private void createFoldersAndUpdateImageFromPathTest(string CompleteimagePath)
    {
        string imageFileName = Path.GetFileName(CompleteimagePath);
        string serialNumber = Regex.Match(imageFileName, @"FE2\d{5}A[HV]\d[0-9]+").Value;
        string date = Regex.Match(imageFileName, @"\d{8}").Value;
        string year = date.Substring(0, 4);
        string month = date.Substring(4, 2);
        string day = date.Substring(6, 2);        
        string newRootPath = @"C:\Public\ImagenTest";
        string newPath = Path.Combine(newRootPath, year, month, day);
        if (!Directory.Exists(newPath))
        {
            Directory.CreateDirectory(newPath);
        }
        //copy image to new path
        string newImagePath = Path.Combine(newPath, imageFileName);
        //force to overwrite if exists
                
        File.Copy(CompleteimagePath, newImagePath);        
    }

   

    private void  updateMultiplesImagesFromPathParallelTest(string rootPath)
    {
        List<string> files = Directory.GetFiles(rootPath, "*.*", SearchOption.AllDirectories).ToList();
        Parallel.ForEach(files, file =>
        {
            if (validateSerialNumberFromImagePathTest(file))
            {
                createFoldersAndUpdateImageFromPathTest(file);
            }
        });
    }

    [Theory]
    [InlineData(@"\\mxgdld0nsifsn03\GDL_TE_VOL_1\TE_Customers\Tesla\Image Files\1735623-01-B_TOP")]
    private void getListofImagesTest(string rootPath)
    {
        
        List<string> files = Directory.GetFiles(rootPath, "*.*", SearchOption.AllDirectories).ToList();
        foreach (string file in files)
        {
           if( validateSerialNumberFromImagePathTest(file))
           {
                createFoldersAndUpdateImageFromPathTest(file);
           }
           
        }
    }
    [Theory]
    [InlineData(@"\\mxgdld0nsifsn03\GDL_TE_VOL_1\TE_Customers\Tesla\Image Files\1735623-01-B_TOP")]
    private void checkConnectionToDBTest(string noseusa)
    {
        string connectionString = "Server=AWUEA1GDLSQL41;Database=TE_ImageRepository;Trusted_Connection=True;";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            SqlCommand command = new SqlCommand("SELECT * FROM TE_ImageRepository.dbo.ImageRepository", connection);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine(String.Format("{0}, {1}", reader[0], reader[1]));
            }
        }
    }
    private void getAllImagesFromPathAndCreateZipDownloadTest(string rootPath)
    {
        
        List<string> files = Directory.GetFiles(rootPath, "*.*", SearchOption.AllDirectories).ToList();
       //copy files to new folder
        string newRootPath = @"C:\Public\ImagenTest";
        string newPath = Path.Combine(newRootPath, "AllImages");
        if (!Directory.Exists(newPath))
        {
            Directory.CreateDirectory(newPath);
        }
        foreach (string file in files)
        {
            string imageFileName = Path.GetFileName(file);
            string newImagePath = Path.Combine(newPath, imageFileName);
            File.Copy(file, newImagePath);
        }
        //create zip file
        string zipPath = Path.Combine(newRootPath, "AllImages.zip");
        ZipFile.CreateFromDirectory(newPath, zipPath);
        

       
       
    }
   
}