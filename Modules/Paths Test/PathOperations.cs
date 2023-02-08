using System.Text.RegularExpressions;


namespace TestWebApi.Modules.Paths_Test;

public class PathOperationsTests
{

    [Theory]
    [InlineData(@"\\mxgdld0nsifsn03\GDL_TE_VOL_1\TE_Customers\Tesla\Image Files\1735623-01-B_TOP\20221215\20221215135251-FE222349AH5139.JPG")]
    public void getDateAndSerialFromPathTest2(string CompleteimagePath)
    {
        string imageFileName = Path.GetFileName(CompleteimagePath);
        //The serial number starts with FE2 then 5 digits, then A then first leter of orientation(horizontal or vertical) then any digit with 
        string serialNumber = Regex.Match(imageFileName, @"FE2\d{5}A[HV]\d[0-9]+").Value;
        string date = Regex.Match(imageFileName, @"\d{8}").Value;
        
        //pattern regex to get datetime  and get serial number
        var serialandDate = Regex.Match(imageFileName, @"(?<date>\d{8})(?<serialNumber>FE2\d{5}A[HV]\d[0-9]+)");
        string serialNumber2 = serialandDate.Groups["serialNumber"].Value;
        string date2 = serialandDate.Groups["date"].Value;


        DateTime dateFromString;
        DateTime.TryParse(date,  out dateFromString);
       
        Assert.True(serialNumber.Length >0);
         Assert.True(dateFromString != null);
        
    }
}