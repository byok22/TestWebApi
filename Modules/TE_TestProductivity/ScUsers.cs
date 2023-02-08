
using System;
using System.Collections.Generic;
namespace TestWebApi;

public class ScUsersTest
{

   

   
    [Theory]
    [InlineData(1)]
    [InlineData(4)]
    public int getById(int id)
    {
      
     return id;
    }
    
   
}