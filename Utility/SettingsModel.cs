public class MySettingsModel  
    {  
        //Application Settings
        public string URL{ get; set;}
        public string APIURL {get; set;}
        public string DbConnection { get; set; } 
        public string Secret { get; set; } 


        //Email Settings
        public string SMTPServer { get; set; }  
        public int SMTPPort { get; set; }  
        public string SMTPUsername { get; set; }  
        public string SMTPPassword { get; set; }  
        
    }