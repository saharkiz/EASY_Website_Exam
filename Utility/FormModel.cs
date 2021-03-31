public class FormModel
    {
        //Common Input
        public string id{get; set;}
        public string customer { get; set; }

        //Form Item
        public string email { get; set; }
        public string password { get; set; }
        public string inputcode{get; set;}
        public string inputmnum{get; set;}
        public string inputName{get; set;}
        public string inputLicense{get; set;}
        public string inputPlate{get; set;}

        //Form Verify
        public string inputmnum1 { get; set; }
        public string inputmnum2 { get; set; }
        public string inputmnum3 { get; set; }
        public string inputmnum4 { get; set; }
        public string inputmnum5 { get; set; }
        public string inputmnum6 { get; set; }
        public string mobile { get; set; }

        //Form Payment
        public string inputCCName { get; set; }
        public string inputCCNum { get; set; }
        public string inputyear { get; set; }
        public string inputmonth { get; set; }
    }