// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
var dataURLProductList = 'http://localhost:5000/app/product/list';
var dataURLProductItem = 'http://localhost:5000/app/product/item/';
var dataURLCardAdd = 'http://localhost:5000/app/card/add/';
var dataURLUserAdd = 'http://localhost:5000/app/user/add/';
var dataURLSendOTP = 'http://localhost:5000/app/sendOTP';
var dataURLValidateOTP = 'http://localhost:5000/app/validateOTP';

if(document.getElementById("vueProductList")){
    var App = new Vue({
        el: '#vueProductList',
        data: {
          productList: []
        },
        mounted() { 
          var self = this
          $.getJSON(dataURLProductList, function(data) {
            self.productList = data;
          });
        }
      })  
}
if(document.getElementById("vueProductPurchase")){
    var App = new Vue({
        el: '#vueProductPurchase',
        data: {
            itemid : 0,
            productItem : {},
            seenformCustomer : true,
            seenformVerify : false,
            seenformPayment : false,
            seenformReview : false,
            seenformDone : false,
            step : 1,
            todaydate : new Date().toLocaleString(),
            //customer
            ccode: "",
            mobile: "",
            otp1 : "",
            otp2 : "",
            otp3 : "",
            otp4 : "",
            otp5 : "",
            otp6 : "",
            //Payment
            cc : "",
            //Review
            subscription : "NA",
            startDate : "TODAY",
            status : "DECLINED",
        },
        mounted() { 
          var self = this
          self.itemid = this.$refs.refItemID.value;
          $.getJSON(dataURLProductItem + self.itemid, function(data) {
            self.productItem = data[0];
          });
        },
        methods: {
            hideall(){
                  this.seenformCustomer = false;
                  this.seenformVerify = false;
                  this.seenformPayment = false;
                  this.seenformReview = false;
                  this.seenformDone = false;
            },
            prev() {
              if (this.step < 6) 
              {
                  this.step = parseInt(this.step) - 1;
                  this.hideall();
                  switch(this.step)
                  {
                          case 1: this.seenformCustomer = true; break;
                          case 2: this.seenformVerify = true; break;
                          case 3: this.seenformPayment = true; break;
                          case 4: this.seenformReview = true; break;
                  }
                }
            },
            saveCustomer()
            {
                const requestOptions = {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify({ mobile: "+"+ this.ccode + this.mobile })
                  };
                  fetch(dataURLSendOTP, requestOptions)
                    .then(response => response.json());

                this.step = parseInt(this.step) + 1;
                this.hideall();
                this.seenformVerify = true;

            },
            checkVerify(){
                this.step = parseInt(this.step) + 1;
                this.hideall();
                this.seenformPayment = true;

                const requestOptions = {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify({ mobile: "+"+ this.ccode + this.mobile,
                                            otp: this.otp1 + this.otp2 + this.otp3 + this.otp4 + this.otp5 + this.otp6  })
                  };
                  fetch(dataURLValidateOTP, requestOptions)
                    .then(response => response.json());
            },
            checkPayment(){
                this.step = parseInt(this.step) + 1;
                this.hideall();
                this.seenformReview = true;
            },
            checkReview(){
                this.step = parseInt(this.step) + 1;
                this.hideall();
                this.seenformDone = true;

                this.subscription = "SUBSCRIBED";
                this.startDate = "TODAY";
                this.status = "ACTIVE";
            },
            }
      })
}