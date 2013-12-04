using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.ComponentModel;

namespace GoogleAnalyticsTracking
{
    public class Tracker
    {
        BackgroundWorker mainBackgroundWorker = new BackgroundWorker();
        private string _urlData;
        private string _googleAnalyticsVersion = "v=1";
        private string _gameVersion = "1";
        private string _trackingID;
        private string _gameName;
        private string _ucid;
        private string _url = "http://www.google-analytics.com/collect";
        bool isFinished = true;
        public int _ProcessPercentage;

        private static Tracker instance = null;

        public static Tracker Instance()
        {
           return Tracker.instance;
        }

        /// <summary>
        /// Setup Google tracking 
        /// </summary>
        /// <param name="googleID">Need google's Tracking ID eg:UA-XXXX-Y  </param>
        /// <param name="appName">Set app name</param>
        /// <param name="gameVersion">Set Game Version Number. Use null to set Game Version to 1</param>
        public Tracker(string googleID,string appName, Nullable<string> gameVersion) {
            if (instance.Equals(null))
            {
                this._trackingID = googleID;
                this._gameName = appName;
                if (gameVersion != null)
                {
                    this._gameVersion = "" + gameVersion;
                }
                instance = this;
            }
        }

        /// <summary>
        /// App Event Tracking
        /// </summary>
        /// <param name="category">Specifies the event category. Must not be empty. ( ec=Category )</param>
        /// <param name="action">Specifies the event action. Must not be empty. ( ea=Action )</param>
        /// <param name="label">Specifies the event label. It can be null</param>
        /// <param name="value">Specifies the event value. Values must be non-negative. It can be null</param>
        public void TrackObject(string category, string action, Nullable<string> label, Nullable<int> value)
        {
            string urlParams;
            urlParams = _googleAnalyticsVersion;
            urlParams += "&tid=" + _trackingID;
            urlParams += "&cid=" + _ucid;
            urlParams += "&an=" + _gameName;
            urlParams += "&t=event";
            urlParams += "&ec=" + category;
            urlParams += "&ea=" + action;
            if (label != null) {
                urlParams += "&el=" + label;
            }
            if (value != null){
                urlParams += "&ev=" + (int)value;
            }
            SendFrom(urlParams);
        }
        
        /// <summary>
        /// Mobile App / Screen Tracking
        /// </summary>
        /// <param name="screen">Specifies the Screen Name of the game hit</param>
        public void TrackObject(string screen)
        {
            string urlParams;
            urlParams = _googleAnalyticsVersion;
            urlParams += "&tid=" + _trackingID;
            urlParams += "&cid=" + _ucid;
            urlParams += "&an=" + _gameName;
            urlParams += "&t=appview";
            urlParams += "&cd=" + screen;
            SendFrom(urlParams);

        }

        /// <summary>
        /// Track exceptions
        /// </summary>
        /// <param name="exceptionDescription">Description of error</param>
        /// <param name="fatal">Was exception fatal</param>
        public void TrackObject(string exceptionDescription, bool fatal)
        {
            string urlParams;
            urlParams = _googleAnalyticsVersion;
            urlParams += "&tid=" + _trackingID;
            urlParams += "&cid=" + _ucid;
            urlParams += "&an=" + _gameName;
            urlParams += "&t=exception";
            urlParams += "&exd=" + exceptionDescription;
            if (fatal) { urlParams += "&exf=1"; }
            else
            {
                urlParams += "&exf=0";
            }
            SendFrom(urlParams);
        }

        /// <summary>
        /// Tracking Social
        /// </summary>
        /// <param name="network">A string representing the social network being tracked (e.g. Facebook, Twitter, LinkedIn)</param>
        /// <param name="action"> A string representing the social action being tracked (e.g. Like, Share, Tweet)</param>
        /// <param name="interaction">Specifies the target of a social interaction. This value is typically a URL but can be any text.</param>
        public void TrackObject(string network, string action, string interaction)
        {
            string urlParams;
            urlParams = _googleAnalyticsVersion;
            urlParams += "&tid=" + _trackingID;
            urlParams += "&cid=" + _ucid;
            urlParams += "&an=" + _gameName;
            urlParams += "&t=social";
            urlParams += "&sn=" + network;
            urlParams += "&sa=" + action;
            urlParams += "&st=" + interaction;
            SendFrom(urlParams);
        }

        /// <summary>
        /// Ecommerce Tracking hit item
        /// </summary>
        /// <param name="transactionID">transaction ID. Required.</param>
        /// <param name="itemName">Item name. Required.</param>
        /// <param name="itemPrice">Price of Item</param>
        /// <param name="quantity">Quantity of items</param>
        /// <param name="itemCode"> Unique Item Code assigned</param>
        /// <param name="itemCategory">Category the item is part of.</param>
        public void TrackObject(string transactionID, string itemName, string itemPrice, string quantity, string itemCode, string itemCategory)
        {
            string urlParams;
            urlParams = _googleAnalyticsVersion;
            urlParams += "&tid=" + _trackingID;
            urlParams += "&cid=" + _ucid;
            urlParams += "&an=" + _gameName;
            urlParams += "&t=item";
            urlParams += "&ti=" + transactionID;
            urlParams += "&in=" + itemName;
            urlParams += "&ip=" + itemPrice;
            urlParams += "&iq=" + quantity;
            urlParams += "&ic=" + itemCode;
            urlParams += "&iv=" + itemCategory;
            urlParams += "&cu=EUR";
            SendFrom(urlParams);
        }

        /// <summary>
        /// Ecommerce Transaction hit
        /// </summary>
        /// <param name="transactionID">ID of the transaction. Required.</param>
        /// <param name="transactionAffiliation">Specifies the affiliation or store name.</param>
        /// <param name="transactionRevenue">Specifies the total revenue associated with the transaction. This value should include any shipping or tax costs.</param>
        /// <param name="transactionShipping">Specifies the total shipping cost of the transaction.</param>
        /// <param name="transactionTax">Specifies the total tax of the transaction.</param>
        public void TrackObject(string transactionID, string transactionAffiliation, string transactionRevenue, string transactionShipping, string transactionTax)
        {
            string urlParams;
            urlParams = _googleAnalyticsVersion;
            urlParams += "&tid=" + _trackingID;
            urlParams += "&cid=" + _ucid;
            urlParams += "&an=" + _gameName;
            urlParams += "&t=item";
            urlParams += "&ti=" + transactionID;
            urlParams += "&ta=" + transactionAffiliation;
            urlParams += "&tr=" + transactionRevenue;
            urlParams += "&ts=" + transactionShipping;
            urlParams += "&tt=" + transactionTax;
            urlParams += "&cu=EUR";
            SendFrom(urlParams);
        }

        void SendFrom(string urlParams) {
            if (!mainBackgroundWorker.IsBusy)
            {
                isFinished = false;
                this._urlData = urlParams;
                mainBackgroundWorker.DoWork += new DoWorkEventHandler(SentData);
                mainBackgroundWorker.ProgressChanged += DoProgressChanged;
                mainBackgroundWorker.RunWorkerCompleted += WorkerCompleted;
                mainBackgroundWorker.RunWorkerAsync();
                mainBackgroundWorker.WorkerReportsProgress = true;
                mainBackgroundWorker.WorkerSupportsCancellation = true;
            }
        }

        void SentData(object sender, DoWorkEventArgs e)
        {
            if (Connected())
            {
                BackgroundWorker worker = sender as BackgroundWorker;
                try
                {
                    
                    System.Threading.Thread.Sleep(50);
                    worker.ReportProgress(20);

                    string address = _url;
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_url);
                    request.KeepAlive = true;
                    request.ProtocolVersion = HttpVersion.Version11;
                    request.Method = "POST";

                    System.Threading.Thread.Sleep(50);
                    worker.ReportProgress(40);

                    byte[] byteArray = Encoding.UTF8.GetBytes(this._urlData);

                    System.Threading.Thread.Sleep(50);
                    worker.ReportProgress(50);

                    request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = byteArray.Length;
                    Stream dataStream = request.GetRequestStream();
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Close();

                    System.Threading.Thread.Sleep(50);
                    worker.ReportProgress(80);

                    WebResponse response = request.GetResponse();
                    dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    string responseFromServer = reader.ReadToEnd();

                    reader.Close();
                    dataStream.Close();
                    response.Close();
                    request.KeepAlive = false;
                    System.Threading.Thread.Sleep(50);
                    worker.ReportProgress(100);
                }
                catch {
                    worker.ReportProgress(100);
                }
            }
            else { 
                //need to store data 
            }
        }

        void WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            isFinished = true;
        }

        void DoProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this._ProcessPercentage = e.ProgressPercentage;
        }

        bool Connected()
        {

            try
            {
                string myAddress = "www.google.com";
                IPAddress[] addresslist = Dns.GetHostAddresses(myAddress);

                if (addresslist[0].ToString().Length > 6)
                {
                    return true;
                }
                else
                    return false;

            }
            catch
            {
                return false;
            }

        }

    }
}
