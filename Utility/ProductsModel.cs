using System;
namespace EASY.Website.Models  
{  
    //OLD SCHOOL Models
    [Serializable]
    public class Product 
    {  
        public int ProductID { get; set; }  
        public string Name { get; set; }
        
        public decimal BasePrice { get; set; }
        public string HtmlDescription{get; set;}
    }  
}  