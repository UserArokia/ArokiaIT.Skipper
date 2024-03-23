using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading;
using ArokiaIT.Framework.Architecture;
using ArokiaIT.OneController.Common.Contract.DataContracts;
using ArokiaIT.Skipper.Business.MobileApp;
using ArokiaIT.Skipper.Common.Contract.DataContracts;
using ArokiaIT.Skipper.Common.Contract.DataContracts.api;
using ArokiaIT.Skipper.Common.Contract.Enumerations;
using ArokiaIT.Skipper.Common.Contract.ServiceContracts;
using ArokiaIT.Skipper.Common.Utility;
using ArokiaIT.Skipper.Common.Utility.AlertUtility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Reporting.WebForms;
using QRCoder;
using APIcredentialsresponse = ArokiaIT.Skipper.Common.Contract.DataContracts.api.APIcredentialsresponse;
using YouGotaGiftOrder = ArokiaIT.Skipper.Common.Contract.DataContracts.api.YouGotaGiftOrder;
using SureGiftRequest = ArokiaIT.Skipper.Common.Contract.DataContracts.api.SureGiftRequest;
using OrderItemRequest = ArokiaIT.Skipper.Common.Contract.DataContracts.api.OrderItemRequest;

namespace ArokiaIT.Skipper.MobileService
{
    /// <summary>
    /// Author:Gaurav
    /// Date : 23-3-2015
    /// Service for mobile
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class CustomerMobileApi : IMobileApi
    {
        #region Variables
        private const string SERVICE_CLASS_NAME = "CustomerMobileServives.svc.cs";
        private const string CUSTOMER_NAME = "[client name]";
        private const string GIFTERS_NAME = "[Gifter’s name]";
        private const string RECIPIENT_NAME = "[recipient name]";
        private const string E_VOUCHER_NAME = "[eVoucher name]";
        private const string CARD_NUMBER = "[card #]";
        private const string PIN = " [PIN]";
        private const string SAVED_PIN = "Saved PIN";
        private const string TRANSWER_POINTS = "[Transfer points]";
        private const string POINTS_IN_ACCOUNT = "[PointsinAccount]";
        private string SENDER_POINTS = string.Empty;
        private string RECEIPENT_POINTS = string.Empty;
        private string TRANSFER_POINTS = string.Empty;
        private const string CHECK_LOLALTYID_EXISTS = "CheckLoyaltyIdExists";
        private const string TO_NAME = "[Client name]";
        private const string ENQUIRY_EMAIL = "EnquiryEmail";
        private const string ENQUIRY_SUBJECT = "EnquirySubject";
        private const string PAGE_PREFIX = "CRP";
        private const string BALANCE = "[BALANCE]";
        private const string SAVE_LOG_DETAILS = "SaveLogDetails";
        private const string ReferralName = "[Referral name]";
        private const string RefereeName = "[Referee Name]";
        private const string NoOfPoints = "[No. of points] ";
        private const string CurrentPoint = "[Current Point Balance]";
        private const string Pin = "[Pin]";
        private const string Key = "workingkey=";
        private const string To = "&to=";
        private const string Sender = "&sender=";
        private const string Message = "&message=";
        private const string SUCCESS = "success";
        private const string PENDING = "pending";
        private const string FAILURE = "failure";
        string Mobile, OperatorCode = string.Empty;
        string actualResponse = string.Empty;
        int CircleId = 0, Amount = 0;
        private const string AUTHRIZEDNO = "[Authrizedno]";
        private const string MEMBER_NAME = "[Member Name]";
        private const string ClientName = "[CustomerName]";
        private const string CLAIM_ID = "[ClaimID]";
        private const string PROMO_NAME = "[PromotionName]";
        private const string LOGIN = "Login";
        private const string ImageDirectoryPathSign = @"~\UploadFiles\CustomerImage\";
        private const string PROFILE_PIC_FOLDER_PATH = @"~\UploadFiles\CustomerImage\";
        private const string RECEIPT_IMAGE_FOLDER_PATH = @"~\UploadFiles\ReceiptImage\";
        private const string IDENTITY_PROOF_PIC_FOLDER_PATH = @"~\UploadFiles\IdentificationProof\";
        private const string CLIENT_NAME = "[client name]";
        private const string POINTS = "[points]";
        private const string REGISTRATION = "Registration";
        private static string headers = string.Empty;
        private static string ResponseString = string.Empty;
        string Token = string.Empty, TokenSecrete = string.Empty;
        private const string PROGRAM_BAHAVIOUR_IMAGE = @"\UploadFiles\BahaviourActivities\";
        private const string REVIEW_VIDEO_PATH = @"~\UploadFiles\Videos\ReviewVideos\";
        private const string NAME = "[Name]";
        private const string MEMBER_ID = "[MemberID]";
        private const string POINT_BALANCE = "[PointBalance]";
        private const string TITLE = "[Title]";
        private const string FIRST_NAME = "[FirstName]";
        private const string LAST_NAME = "[LastName]";
        string QRCodeID = string.Empty;
        public const string SCRATCH_CODE_INVALID = "SC_Invalid";
        public const string SCRATCH_CODE_USED = "SC_Used";
        public const string SCRATCH_CODE_BUSY = "SC_Busy";
        public const string SCRATCH_CODE_VALID = "SC_Valid";
        string CustomerID, CustomerName, CustomerEmail = string.Empty;

        private string MERCHANT_EMAIL_ID = ConfigurationManager.AppSettings["MerchantMailID"];
        private string MERCHANT_MOBILE_NO = ConfigurationManager.AppSettings["MerchantMobileNo"];
        private string MERCHANT_USERNAME = ConfigurationManager.AppSettings["MerchantUsername"];
        private static string DOMAIN = ConfigurationManager.AppSettings["Domain"];
        private string workingkey = ConfigurationManager.AppSettings["workingkey"];
        private string smssender = ConfigurationManager.AppSettings["sender"];
        private string TOKEN_KEY_URL = ConfigurationManager.AppSettings["TokenKeyUrl"];
        private string RECHARGE_API_URL = ConfigurationManager.AppSettings["RechargeApiUrl"];
        private string CHECK_BILLE_API_URL = ConfigurationManager.AppSettings["CheckBillApiUrl"];
        private string API_USER_NAME = ConfigurationManager.AppSettings["ApiUserName"];
        private string API_PASSWORD = ConfigurationManager.AppSettings["ApiPassword"];
        private string SECREATE_KEY = ConfigurationManager.AppSettings["SecreateKey"];
        private string API_MAIL_ID = ConfigurationManager.AppSettings["ApiMailID"];
        private string CLIENT_ID = ConfigurationManager.AppSettings["ClientID"];
        private readonly int VoucherVendorId = Convert.ToInt32(ConfigurationManager.AppSettings["VoucherVendorId"]);
        private readonly string VoucherVendor = ConfigurationManager.AppSettings["VoucherVendor"];
        private string BRAND_NAME = ConfigurationManager.AppSettings["BrandName"];
        private string LOCATION_EMAILID = ConfigurationManager.AppSettings["LocationEmailID"];
        private string LOCATION_MOBILE_NUM = ConfigurationManager.AppSettings["LocationMobileNum"];
        private static string HTTP_UPLOAD_FOLDER_PATH = ConfigurationManager.AppSettings["UPLOAD_FOLDER_PATH"];
        private const string UPDATE_CHANGED_PASSWORD = "UpdateChangedPassword";
        private string APIURL = ConfigurationManager.AppSettings["APIURL"];
        private static string OauthUsername = ConfigurationManager.AppSettings["OauthUsername"];
        private static string OauthPassword = ConfigurationManager.AppSettings["OauthPassword"];
        private static string QWICK_CILVER_ORDER_API = ConfigurationManager.AppSettings["QWICK_CILVER_ORDER_API"];
        private static string GOOGLE_API_KEY = ConfigurationManager.AppSettings["GoogleAPIKey"];
        private static string QWICK_CILVER_RESEND_API = ConfigurationManager.AppSettings["QWICK_CILVER_RESEND_API"];
        private static string QWICK_CILVER_STATUS_API = ConfigurationManager.AppSettings["QWICK_CILVER_STATUS_API"];
        private static string API_KEY = ConfigurationManager.AppSettings["APIkey"];




        private static string SURE_GIFT_BALANCE_API_K = ConfigurationManager.AppSettings["SURE_GIFT_BALANCE_API_K"];
        private static string SURE_GIFT_BALANCE_API_N = ConfigurationManager.AppSettings["SURE_GIFT_BALANCE_API_N"];

        private static string rdlcReportPath = HTTP_UPLOAD_FOLDER_PATH + @"\RDLCReports\AssesmentCertificate.rdlc";

        private const string ExportTableSOA = "SOA";
        private const string ExportTableCodeHistory = "ScanCodeHistory";
        private const string ExportTableRewardTransactionPDF = "RewardTransactionPDF";
        private const string ExportTableCustomerEarnedDetailsExcel = "CustomerEarnedDetailsExcelReport";
        private static string rdlcRewardTransactionPDF = HTTP_UPLOAD_FOLDER_PATH + @"\RDLCReports\RewardTransactionPDF.rdlc";
        private static string rdlcReportCodeHistory = HTTP_UPLOAD_FOLDER_PATH + @"\RDLCReports\QrCodeScanHistory.rdlc";
        private static string rdlcReportSOA = HTTP_UPLOAD_FOLDER_PATH + @"\RDLCReports\PDFReportsForGoodPack.rdlc";
        private static string rdlcReportCustomerEarnedDetails = HTTP_UPLOAD_FOLDER_PATH + @"\RDLCReports\CustomerPointsEarnedDetailsForMobile.rdlc";
        private static string SURE_GIFT_TEMPLATE_ID_K = ConfigurationManager.AppSettings["SURE_GIFT_TEMPLATE_ID_K"];
        private static string SURE_GIFT_TEMPLATE_ID_N = ConfigurationManager.AppSettings["SURE_GIFT_TEMPLATE_ID_N"];
        private static string SURE_GIFT_ORDER_API_K = ConfigurationManager.AppSettings["SURE_GIFT_ORDER_API_K"];
        private static string SURE_GIFT_ORDER_API_N = ConfigurationManager.AppSettings["SURE_GIFT_ORDER_API_N"];
        private static string GST_API_KEY = ConfigurationManager.AppSettings["GstApiKey"];
        private static string GST_SECURITY_KEY = ConfigurationManager.AppSettings["GstSecurityKey"];
        private static string TOKEN_URL = ConfigurationManager.AppSettings["GstTokenUrl"];
        private static string GST_DETAIL_URL = ConfigurationManager.AppSettings["GstDetailURL"];
        MobileAppBO mobileAppBO = new MobileAppBO();
        GoogleTranslate google = new GoogleTranslate(GOOGLE_API_KEY);
        private static string FileParameterPath = ConfigurationManager.AppSettings["FileParameter"];
        private const string BANK_PIC_FOLDER_PATH = @"~\UploadFiles\BankImageUpload\";

        #endregion

        #region Customer_Registration
        public CustomerSaveResponse SaveCustomerRegistrationDetailsMobileApp(CustomerSaveRequest ObjCustomerSaveRequest)
        {
            CustomerSaveResponse objCustomerSaveResponse = null;
            try
            {
                var FileName = string.Empty; string PlainPassword = string.Empty; string FullPath = string.Empty;
                if (DOMAIN == UIConstants.NCL || DOMAIN == UIConstants.CHAWLA_MOTORS || DOMAIN == UIConstants.GREEN_COVER || DOMAIN == UIConstants.BUDWEISER || DOMAIN == UIConstants.SANGHI
                    || DOMAIN == UIConstants.VOLVO || DOMAIN == UIConstants.GVLC || DOMAIN == UIConstants.ATMB || DOMAIN == UIConstants.CPML || DOMAIN == UIConstants.MILLER_CLUB || DOMAIN == UIConstants.SAFALTA_QUBA || DOMAIN == UIConstants.WAVIN || DOMAIN == UIConstants.WALK_KARO)
                {
                    if (!string.IsNullOrEmpty(ObjCustomerSaveRequest.ObjCustomer.DisplayImage))
                    {
                        FullPath = HTTP_UPLOAD_FOLDER_PATH + PROFILE_PIC_FOLDER_PATH;
                        FileName = ObjCustomerSaveRequest.ObjCustomer.LoyaltyId + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".png";
                        System.Drawing.Image CustomerImage = UICommon.Base64ToImageConverter(ObjCustomerSaveRequest.ObjCustomer.DisplayImage);
                        CustomerImage.Save((FullPath.Replace("~", "") + FileName), ImageFormat.Png);
                        ObjCustomerSaveRequest.ObjCustomer.DisplayImage = FileName;
                    }
                    if (ObjCustomerSaveRequest.lstIdentityInfo != null)
                    {
                        foreach (CustomerIdentityInfo objCustomerIdentityInfo in ObjCustomerSaveRequest.lstIdentityInfo)
                        {
                            string IdProofFolderPath = HTTP_UPLOAD_FOLDER_PATH + IDENTITY_PROOF_PIC_FOLDER_PATH;
                            string imageName = ObjCustomerSaveRequest.ObjCustomer.LoyaltyId + objCustomerIdentityInfo.IdentityID + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".png";
                            bool Result = UICommon.SaveBase64StringIntoFile(IdProofFolderPath.Replace("~", ""), objCustomerIdentityInfo.IdentityDocument, imageName);
                            objCustomerIdentityInfo.IdentityDocument = imageName;
                        }
                    }
                    if (DOMAIN == UIConstants.BUDWEISER)
                    {
                        PlainPassword = ObjCustomerSaveRequest.ObjCustomer.Password;
                        ObjCustomerSaveRequest.ObjCustomer.LoyaltyId = ObjCustomerSaveRequest.ObjCustomer.Email;
                        ObjCustomerSaveRequest.ObjCustomer.LoyaltyIdAutoGen = false;
                    }
                    else
                    {
                        PlainPassword = UICommon.GetRandomNumber();
                    }
                    ObjCustomerSaveRequest.ObjCustomer.Password = Security.EncryptPassword(PlainPassword);

                    if (!string.IsNullOrEmpty(ObjCustomerSaveRequest.ObjCustomer.JDOB) && (ObjCustomerSaveRequest.ObjCustomer.JDOB != Convert.ToString("1/1/0001")))
                        ObjCustomerSaveRequest.ObjCustomer.DOB = Convert.ToDateTime(ObjCustomerSaveRequest.ObjCustomer.JDOB);
                    if (!string.IsNullOrEmpty(ObjCustomerSaveRequest.ObjCustomer.JAnniversary) && (ObjCustomerSaveRequest.ObjCustomer.JAnniversary != Convert.ToString("1/1/0001")))
                        ObjCustomerSaveRequest.ObjCustomer.Anniversary = Convert.ToDateTime(ObjCustomerSaveRequest.ObjCustomer.JAnniversary);
                }
                ObjCustomerSaveRequest.ObjCustomer.Domain = DOMAIN;
                objCustomerSaveResponse = mobileAppBO.SaveCustomerRegistrationDetailsMobileApp(ObjCustomerSaveRequest);

                if (DOMAIN == UIConstants.NCL || DOMAIN == UIConstants.CHAWLA_MOTORS || DOMAIN == UIConstants.GREEN_COVER || DOMAIN == UIConstants.BUDWEISER || DOMAIN == UIConstants.SANGHI || DOMAIN == UIConstants.ATMB || DOMAIN == UIConstants.CPML || DOMAIN == UIConstants.MILLER_CLUB || DOMAIN == UIConstants.SAFALTA_QUBA || DOMAIN == UIConstants.WAVIN || DOMAIN == UIConstants.WALK_KARO
                    )
                {
                    if (ObjCustomerSaveRequest.ActionType == 0)
                    {
                        if (objCustomerSaveResponse.ReturnMessage.Contains("~"))
                        {
                            if (objCustomerSaveResponse.ReturnMessage.Split('~')[0].ToString() == "1")
                            {
                                if (objCustomerSaveResponse.ReturnMessage.Split('~')[1].ToString() != "-2" && objCustomerSaveResponse.ReturnMessage.Split('~')[1].ToString() != "-3")
                                {
                                    if (DOMAIN != UIConstants.VOLVO)
                                    {
                                        if (objCustomerSaveResponse.ReturnMessage.Split('~')[1].ToString() != "")
                                        {
                                            if (DOMAIN == UIConstants.SAFALTA_QUBA || DOMAIN == UIConstants.MILLER_CLUB)
                                            {
                                                SendWelcomeAccountActivationSMS(ObjCustomerSaveRequest.ObjCustomer.FirstName, ObjCustomerSaveRequest.ObjCustomer.Mobile);
                                            }
                                            else
                                            {
                                                SendCustRegCredentials(ObjCustomerSaveRequest.ObjCustomer.LoyaltyId, ObjCustomerSaveRequest.ObjCustomer.FirstName,
                                                PlainPassword, ObjCustomerSaveRequest.ObjCustomer.Mobile, objCustomerSaveResponse.ReturnMessage.Split('~')[1].ToString());
                                                if (DOMAIN == UIConstants.BUDWEISER)
                                                    SendCustRegCredentialsEmail(ObjCustomerSaveRequest.ObjCustomer.LoyaltyId, ObjCustomerSaveRequest.ObjCustomer.FirstName,
                                                   PlainPassword, ObjCustomerSaveRequest.ObjCustomer.Email, objCustomerSaveResponse.ReturnMessage.Split('~')[1].ToString());
                                                IssueAutomaticGiftcard(objCustomerSaveResponse.ReturnMessage.Split('~')[1].ToString(), REGISTRATION, ObjCustomerSaveRequest.ObjCustomer.RegistrationSource);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "SaveCustomerRegistrationDetailsMobileApp : " + ex.Message);
            }
            return objCustomerSaveResponse;

        }
        #endregion

        #region Customer Dasboard
        public CustomerFeedBackRetriveResponse GetMerchantEmailDetailsMobileApp(CustomerFeedBackRetriveRequest objCustomerFeedBackRetriveRequest)
        {
            CustomerFeedBackRetriveResponse objCustomerFeedBackRetriveResponse = null;
            try
            {
                objCustomerFeedBackRetriveResponse = mobileAppBO.GetMerchantEmailDetailsMobileApp(objCustomerFeedBackRetriveRequest);

                if (objCustomerFeedBackRetriveResponse != null)
                {
                    if (objCustomerFeedBackRetriveResponse.lstCustomerFeedBackJson != null)
                    {
                        if (objCustomerFeedBackRetriveResponse.lstCustomerFeedBackJson.Count > 0)
                        {
                            objCustomerFeedBackRetriveResponse.lstCustomerFeedBackJson[0].LoyaltyIdQRCode = GenerateQrCode(objCustomerFeedBackRetriveResponse.lstCustomerFeedBackJson[0].LoyaltyId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "GetMerchantEmailDetailsMobileApp() : " + ex.Message);
            }

            return objCustomerFeedBackRetriveResponse;
        }
        public CustomerDashboardRetrieveResponse getCustomerDashboardDetailsMobileApp(CustomerDashboardRetrieveRequest ObjCustomerDashboardRetrieveReq)
        {
            CustomerDashboardRetrieveResponse objCustomerDashboardRetrieveResponse = null;
            try
            {
                objCustomerDashboardRetrieveResponse = mobileAppBO.getCustomerDashboardDetailsMobileApp(ObjCustomerDashboardRetrieveReq);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "getCustomerDashboardDetailsMobileApp : " + ex.Message);
            }

            return objCustomerDashboardRetrieveResponse;
        }
        #endregion

        #region Green_Rewards Redemption
        public MerchantRetrieveResponse GetMerchantDetailsMobileApp(MerchantRetrieveRequest ObjMerchantRetrieveRequest)
        {

            MerchantRetrieveResponse ObjMerchantRetrieveResponse = null;
            try
            {
                ObjMerchantRetrieveResponse = mobileAppBO.GetMerchantDetailsMobileApp(ObjMerchantRetrieveRequest);
                if (ObjMerchantRetrieveResponse != null)
                {
                    if (ObjMerchantRetrieveResponse.lstMerchantDetails != null)
                    {
                        foreach (MerchantCosting item in ObjMerchantRetrieveResponse.lstMerchantDetails)
                        {
                            if (!string.IsNullOrEmpty(item.Password))
                                item.Password = UICommon.Decrypt(item.Password);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetMerchantDetailsMobileApp : " + ex.Message);
            }
            return ObjMerchantRetrieveResponse;
        }

        public GiftCardIssueRetrieveResponse getGiftCardIssueMobileApp(GiftCardIssueRetrieveRequest ObjGiftCardIssueRetrieveRequest)
        {
            GiftCardIssueRetrieveResponse objGiftCardIssueRetrieveResponse = null;
            try
            {

                objGiftCardIssueRetrieveResponse = mobileAppBO.getGiftCardIssueMobileApp(ObjGiftCardIssueRetrieveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "getGiftCardIssueMobileApp : " + ex.Message);
            }

            return objGiftCardIssueRetrieveResponse;
        }
        #endregion

        #region My Green Rewards
        public RewardTransactionRetrieveResponse GetRewardTransactionDetailsMobileApp(RewardTransactionRetrieveRequest ObjRewardTransactionRetrieveRequest)
        {
            RewardTransactionRetrieveResponse ObjRewardTransactionRetrieveResponse = null;
            try
            {
                ObjRewardTransactionRetrieveRequest.Domain = DOMAIN;
                ObjRewardTransactionRetrieveResponse = mobileAppBO.GetRewardTransactionDetailsMobileApp(ObjRewardTransactionRetrieveRequest);
                if (ObjRewardTransactionRetrieveRequest.IsPDF)
                {
                    if (ObjRewardTransactionRetrieveResponse != null)
                    {
                        if (ObjRewardTransactionRetrieveResponse.lstRewardTransJsonDetails != null)
                        {
                            if (ObjRewardTransactionRetrieveResponse.lstRewardTransJsonDetails.Count > 0)
                            {
                                var PDF = GetRewardTransactionPdf(ObjRewardTransactionRetrieveResponse.lstRewardTransJsonDetails, ObjRewardTransactionRetrieveRequest);

                                string base64String = Convert.ToBase64String(PDF, 0, PDF.Length);
                                ObjRewardTransactionRetrieveResponse.PDF = base64String;
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "GetRewardTransactionDetailsMobileApp() : " + ex.Message);
            }

            return ObjRewardTransactionRetrieveResponse;
        }
        #endregion

        #region My Redemption
        //Attribute detail for filer by dropdown
        public AttributesRetrieveResponse GetAttributeDetailsMobileApp(AttributesRetrieveRequest objAttributesRetrieveRequest)
        {
            AttributesRetrieveResponse objAttributesRetrieveResponse = null;
            try
            {

                objAttributesRetrieveResponse = mobileAppBO.GetAttributeDetailsMobileApp(objAttributesRetrieveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "GetAttributeDetailsMobileApp : " + ex.Message);
            }

            return objAttributesRetrieveResponse;
        }

        public GiftCardIssueRetrieveResponseJson GetCustomerRedemptionDetailsMobileApp(GiftCardIssueRetrieveRequest ObjGiftCardIssueRetrieveRequest)
        {
            GiftCardIssueRetrieveResponseJson ObjGiftCardIssueRetrieveResponse = null;
            try
            {

                ObjGiftCardIssueRetrieveResponse = mobileAppBO.GetCustomerRedemptionDetailsMobileApp(ObjGiftCardIssueRetrieveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "GetCustomerRedemptionDetailsMobileApp() : " + ex.Message);

            }

            return ObjGiftCardIssueRetrieveResponse;
        }
        #endregion


        #region karthik
        public GiftCardIssueSaveResponseJson UpdateGiftCardTransactionMobileApp(GiftCardIssueSaveRequestJson ObjGiftCardIssueSaveRequest)
        {
            GiftCardIssueSaveResponseJson ObjGiftCardIssueSaveResponse = null;
            try
            {
                string CardPin = GetCardPin(ObjGiftCardIssueSaveRequest.GiftCardIssueDetails.GiftCardIssueId);
                ObjGiftCardIssueSaveResponse = mobileAppBO.UpdateGiftCardTransactionMobileApp(ObjGiftCardIssueSaveRequest);
                sendMailToRecepient(ObjGiftCardIssueSaveRequest.GiftCardHolderInfoDetails.Email, ObjGiftCardIssueSaveRequest.GiftCardHolderInfoDetails.RecipentName, ObjGiftCardIssueSaveRequest.GiftCardIssueDetails.PinType, ObjGiftCardIssueSaveResponse.ReturnMessage, ObjGiftCardIssueSaveRequest.GiftCardIssueDetails.EvoucherName, CardPin);
                sendMailToCardOwner(ObjGiftCardIssueSaveRequest.GiftCardIssueDetails.OwnerEmail, ObjGiftCardIssueSaveRequest.GiftCardHolderInfoDetails.SenderName, ObjGiftCardIssueSaveRequest.GiftCardIssueDetails.PinType, ObjGiftCardIssueSaveRequest.GiftCardIssueDetails.EvoucherName, ObjGiftCardIssueSaveRequest.GiftCardIssueDetails.EvoucherNumber, ObjGiftCardIssueSaveRequest.GiftCardHolderInfoDetails.RecipentName, CardPin);

                sendSMSToRecepient(ObjGiftCardIssueSaveRequest.GiftCardHolderInfoDetails.Mobile, ObjGiftCardIssueSaveRequest.GiftCardIssueDetails.PinType, ObjGiftCardIssueSaveRequest.GiftCardIssueDetails.EvoucherName, ObjGiftCardIssueSaveResponse.ReturnMessage, ObjGiftCardIssueSaveRequest.GiftCardHolderInfoDetails.RecipentName, CardPin);
                sendSMSToCardOwner(ObjGiftCardIssueSaveRequest.GiftCardIssueDetails.CardOwnerMobile, ObjGiftCardIssueSaveRequest.GiftCardIssueDetails.EvoucherName, ObjGiftCardIssueSaveRequest.GiftCardIssueDetails.EvoucherNumber);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "UpdateGiftCardTransactionMobileApp() : " + ex.Message);

            }

            return ObjGiftCardIssueSaveResponse;
        }
        private void sendMailToRecepient(string EmailId, string Name, string PinType, string eVoucherNumber, string eVoucherName, string Pin = "", string SenderName = "")
        {
            try
            {
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                if (PinType == "Saved PIN")
                {
                    ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.GiftaneVoucherwithPIN_ToRecipient);
                }
                else
                {
                    ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.GiftaneVoucherwithoutPINOrwithOTP_ToRecipient);
                }

                ObjSmsEmailRetrieveRequest.Type = UIConstants.EMAIL;
                ObjSmsEmailRetrieveRequest.UserName = MERCHANT_USERNAME;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = EmailId;
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse = mobileAppBO.GetEmailSmsTemplate(ObjSmsEmailRetrieveRequest);
                if ((ObjSmsEmailRetrieveResponse != null) && (ObjSmsEmailRetrieveResponse.lstSmsEmailDetails != null) && (ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count > 0))
                {
                    for (int i = 0; i < ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count; i++)
                    {
                        List<AlertTemplateDynamicContent> DynamicTemplate = new List<AlertTemplateDynamicContent>()
                            {
                            new AlertTemplateDynamicContent(CUSTOMER_NAME, Name),
                            new AlertTemplateDynamicContent(GIFTERS_NAME, SenderName),
                            new AlertTemplateDynamicContent(E_VOUCHER_NAME, eVoucherName),
                            new AlertTemplateDynamicContent(CARD_NUMBER, eVoucherNumber),
                            new AlertTemplateDynamicContent("[table]", "need to send voucher html"),
                            new AlertTemplateDynamicContent(PIN, Pin),
                            };

                        //Sending Email
                        AlertUtiltityParameters alertUtiltityParametersEMAIL = new AlertUtiltityParameters
                                      (UIConstants.EMAIL, EmailId, DynamicTemplate, ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i]);
                        var Result = SendAlertUtility.SendAlert(alertUtiltityParametersEMAIL);
                    }
                }


            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "sendMailToRecepient() : " + ex.Message);
            }
            finally
            { }
        }
        private void sendSMSToRecepient(string Mobile, string PinType, string EvoucherName, string EvoucherNumber, string Recepientname, string Pin = "")
        {
            try
            {
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                if (PinType == UIConstants.SAVED_PIN)
                {
                    ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.GiftaneVoucherwithPIN_ToRecipient);
                }
                else
                {
                    ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.GiftaneVoucherwithoutPINOrwithOTP_ToRecipient);
                }
                ObjSmsEmailRetrieveRequest.Type = UIConstants.SMS;
                ObjSmsEmailRetrieveRequest.UserName = MERCHANT_USERNAME;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = Mobile;
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse = mobileAppBO.GetEmailSmsTemplate(ObjSmsEmailRetrieveRequest);

                if ((ObjSmsEmailRetrieveResponse != null) && (ObjSmsEmailRetrieveResponse.lstSmsEmailDetails != null) && (ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count > 0))
                {
                    for (int i = 0; i < ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count; i++)
                    {
                        List<AlertTemplateDynamicContent> DynamicTemplate = new List<AlertTemplateDynamicContent>()
                            {
                            new AlertTemplateDynamicContent(CUSTOMER_NAME, Recepientname),
                            new AlertTemplateDynamicContent(E_VOUCHER_NAME, EvoucherName),
                            new AlertTemplateDynamicContent(CARD_NUMBER, EvoucherNumber),
                            new AlertTemplateDynamicContent(PIN, Pin)
                            };


                        //Sending SMS
                        AlertUtiltityParameters alertUtiltityParametersSMS = new AlertUtiltityParameters
                                      (ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i].Type, Mobile, DynamicTemplate, ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i]);

                        alertUtiltityParametersSMS.IsGetRequest = true;
                        var SMSResult = SendAlertUtility.SendAlert(alertUtiltityParametersSMS);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "sendSMSToRecepient() : " + ex.Message);
            }
        }
        private void sendMailToCardOwner(string EmailId, string Name, string PinType, string eVoucherName, string eVoucherNumber, string ReceipentName, string Pin = "")
        {
            try
            {
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                if (PinType == "Saved PIN")
                {
                    ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.GiftAneVoucherWithPIN_ToGifter);
                }
                else
                {
                    ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.GiftaneVoucherwithoutPINOrwithOTP_ToGifter);

                    ObjSmsEmailRetrieveRequest.Type = UIConstants.EMAIL;
                    ObjSmsEmailRetrieveRequest.UserName = MERCHANT_USERNAME;
                    ObjSmsEmailRetrieveRequest.EmailOrMobile = EmailId;
                    SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse = mobileAppBO.GetEmailSmsTemplate(ObjSmsEmailRetrieveRequest);
                    if ((ObjSmsEmailRetrieveResponse != null) && (ObjSmsEmailRetrieveResponse.lstSmsEmailDetails != null) && (ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count > 0))
                    {
                        for (int i = 0; i < ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count; i++)
                        {
                            List<AlertTemplateDynamicContent> DynamicTemplate = new List<AlertTemplateDynamicContent>()
                            {
                            new AlertTemplateDynamicContent(CUSTOMER_NAME, Name),
                            new AlertTemplateDynamicContent(RECIPIENT_NAME, ReceipentName),
                            new AlertTemplateDynamicContent(E_VOUCHER_NAME, eVoucherName),
                            new AlertTemplateDynamicContent(CARD_NUMBER, eVoucherNumber),
                            new AlertTemplateDynamicContent(PIN, Pin),
                            };
                            //Sending Email
                            AlertUtiltityParameters alertUtiltityParametersEMAIL = new AlertUtiltityParameters
                                          (UIConstants.EMAIL, EmailId, DynamicTemplate, ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i]);
                            var Result = SendAlertUtility.SendAlert(alertUtiltityParametersEMAIL);
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "sendSMSToRecepient() : " + ex.Message);
            }
            finally
            { }
        }
        private void sendSMSToCardOwner(string Mobile, string EvoucherName, string EvoucherNumber, string name = "")
        {
            try
            {
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.GiftAneVoucherWithPIN_ToGifter);
                ObjSmsEmailRetrieveRequest.Type = UIConstants.SMS;
                ObjSmsEmailRetrieveRequest.UserName = MERCHANT_USERNAME;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = Mobile;
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse = mobileAppBO.GetEmailSmsTemplate(ObjSmsEmailRetrieveRequest);

                if ((ObjSmsEmailRetrieveResponse != null) && (ObjSmsEmailRetrieveResponse.lstSmsEmailDetails != null) && (ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count > 0))
                {
                    for (int i = 0; i < ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count; i++)
                    {
                        List<AlertTemplateDynamicContent> DynamicTemplate = new List<AlertTemplateDynamicContent>()
                            {
                            new AlertTemplateDynamicContent(CUSTOMER_NAME, NAME),
                            new AlertTemplateDynamicContent(E_VOUCHER_NAME, EvoucherName),
                            new AlertTemplateDynamicContent(CARD_NUMBER, EvoucherNumber)
                            };


                        //Sending SMS
                        AlertUtiltityParameters alertUtiltityParametersSMS = new AlertUtiltityParameters
                                      (ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i].Type, Mobile, DynamicTemplate, ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i]);

                        alertUtiltityParametersSMS.IsGetRequest = true;
                        var SMSResult = SendAlertUtility.SendAlert(alertUtiltityParametersSMS);
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public CountryRetrieveResponse GetCountryDetailsMobileApp(CountryRetrieveRequest countryRetrieveRequest)
        {
            CountryRetrieveResponse countryRetrieveResponse = null;
            try
            {

                countryRetrieveResponse = mobileAppBO.GetCountryDetailsMobileApp(countryRetrieveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetCountryDetailsMobileApp : " + ex.Message);
            }

            return countryRetrieveResponse;
        }

        public StateRetrieveResponse GetStateDetailsMobileApp(StateRetrieveRequest stateRetrieveRequest)
        {
            StateRetrieveResponse stateRetrieveResponse = null;
            try
            {

                stateRetrieveResponse = mobileAppBO.GetStateDetailsMobileApp(stateRetrieveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetStateDetailsMobileApp : " + ex.Message);
            }

            return stateRetrieveResponse;
        }

        public CityRetrieveResponse GetCityDetailsMobileApp(CityRetrieveRequest cityRetrieveRequest)
        {
            CityRetrieveResponse CityRetrieveResponse = null;
            try
            {

                CityRetrieveResponse = mobileAppBO.GetCityDetailsMobileApp(cityRetrieveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetCityDetailsMobileApp : " + ex.Message);
            }

            return CityRetrieveResponse;
        }
        #endregion karthik

        #region Is_Authenticate
        public UserLoginRetrieveResponse CheckIsAuthenticatedMobileApp(UserRetrieveRequest_Loyalty userRetrieveRequest)
        {
            UserLoginRetrieveResponse objUserLoginRetrieveResponse = null;
            try
            {
                if (userRetrieveRequest.ActionType == UPDATE_CHANGED_PASSWORD)
                {
                    userRetrieveRequest.UserId = -1;
                    userRetrieveRequest.Password = Security.EncryptPassword(userRetrieveRequest.Password);
                    userRetrieveRequest.UserType = UIConstants.CUSTOMER_TYPE;
                }
                objUserLoginRetrieveResponse = mobileAppBO.CheckIsAuthenticatedMobileApp(userRetrieveRequest);
                if (objUserLoginRetrieveResponse == null)
                {
                    objUserLoginRetrieveResponse = new UserLoginRetrieveResponse();
                    objUserLoginRetrieveResponse.UserList = new List<User_Loyalty_Base>();
                    User_Loyalty_Base objUser_Loyalty_Base = new User_Loyalty_Base();
                    objUserLoginRetrieveResponse.UserList.Add(objUser_Loyalty_Base);
                    objUserLoginRetrieveResponse.UserList[0].Result = -1;
                }
                else if (userRetrieveRequest.ActionType == CHECK_LOLALTYID_EXISTS)
                {
                    objUserLoginRetrieveResponse.UserList[0].Result = 1;
                }

                else if (userRetrieveRequest.ActionType == UPDATE_CHANGED_PASSWORD)
                {
                    if (objUserLoginRetrieveResponse.UserList.Count > 0)
                    {
                        objUserLoginRetrieveResponse.UserList[0].Result = 1;
                    }
                }
                else
                {
                    if (Security.ValidateUser(userRetrieveRequest.Password, objUserLoginRetrieveResponse.UserList[0].Password))
                    {
                        objUserLoginRetrieveResponse.UserList[0].Result = 1;
                        userRetrieveRequest.ActionType = SAVE_LOG_DETAILS;
                        mobileAppBO.CheckIsAuthenticatedMobileApp(userRetrieveRequest);
                        IssueAutomaticGiftcard(userRetrieveRequest.UserName, LOGIN, Convert.ToInt32(UIConstants.VALUE_TWO));
                    }
                    else
                    {
                        objUserLoginRetrieveResponse.UserList[0].Result = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "CheckIsAuthenticatedMobileApp : " + ex.Message);
            }
            return objUserLoginRetrieveResponse;
        }

        public UserLoginRetrieveResponse SaveUserLogDetails(UserRetrieveRequest_Loyalty userRetrieveRequest)
        {
            UserLoginRetrieveResponse objUserLoginRetrieveResponse = new UserLoginRetrieveResponse();
            try
            {
                objUserLoginRetrieveResponse.UserList = new List<User_Loyalty_Base>();
                User_Loyalty_Base objUser_Loyalty_Base = new User_Loyalty_Base();
                objUser_Loyalty_Base.Result = 1;
                objUserLoginRetrieveResponse.UserList.Add(objUser_Loyalty_Base);
                userRetrieveRequest.ActionType = SAVE_LOG_DETAILS;
                mobileAppBO.CheckIsAuthenticatedMobileApp(userRetrieveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "SaveUserLogDetails : " + ex.Message);

            }
            return objUserLoginRetrieveResponse;
        }
        #endregion

        #region SMS & EMAIL

        //Start : Evoucher Gift
        public bool GE_SMSMail_RecepientMobileApp(string EmailId, string Mobile, string CustomerName, string PinType, string eVoucherNumber, string eVoucherName, string MerchantName, string CurrentUserName, int CardIssueID, string Title)
        {
            bool result = default(bool);
            try
            {
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                string Pin = string.Empty;
                if (PinType == SAVED_PIN)
                    Pin = GetCardPin(CardIssueID);

                SmsEmail ObjSmsEmail = new SmsEmail();


                if (PinType == SAVED_PIN)
                    ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.GiftaneVoucherwithPIN_ToRecipient);
                else
                    ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.GiftaneVoucherwithoutPINOrwithOTP_ToRecipient);
                ObjSmsEmailRetrieveRequest.Type = UIConstants.EMAIL;
                ObjSmsEmailRetrieveRequest.UserName = MerchantName;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = EmailId;
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse = mobileAppBO.GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);
                for (int i = 0; i < ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count; i++)
                {
                    List<AlertTemplateDynamicContent> DynamicTemplate = new List<AlertTemplateDynamicContent>() {
                      new AlertTemplateDynamicContent(CUSTOMER_NAME, (Title + CustomerName)),
                      new AlertTemplateDynamicContent(GIFTERS_NAME, MerchantName),
                      new AlertTemplateDynamicContent(E_VOUCHER_NAME, eVoucherName),
                      new AlertTemplateDynamicContent(CARD_NUMBER, eVoucherNumber),
                     };

                    if (PinType == SAVED_PIN)
                    {
                        DynamicTemplate.Add(new AlertTemplateDynamicContent(PIN, Pin));
                    }

                    AlertUtiltityParameters alertUtiltityParameters = new AlertUtiltityParameters
                                  (UIConstants.EMAIL, EmailId, DynamicTemplate, ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i]);

                    alertUtiltityParameters.IsAlertParameterConfiguredFromDB = true;
                    alertUtiltityParameters.MailHeaderText = UIConstants.OTHERS_HEADER;
                    alertUtiltityParameters.ServerDetails = DataBaseSMTP.GetDBSmtp(MerchantName);
                    SendAlertUtility.SendAlert(alertUtiltityParameters);

                }
                // SMS
                ObjSmsEmail = new SmsEmail();
                mobileAppBO = new MobileAppBO();
                ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                if (PinType == SAVED_PIN)
                    ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.GiftaneVoucherwithPIN_ToRecipient);
                else
                    ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.GiftaneVoucherwithoutPINOrwithOTP_ToRecipient);
                ObjSmsEmailRetrieveRequest.Type = UIConstants.SMS;
                ObjSmsEmailRetrieveRequest.UserName = MerchantName;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = Mobile;
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponseSMS = mobileAppBO.GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);
                {
                    StringBuilder templat = new StringBuilder();
                    templat.Append(ObjSmsEmailRetrieveResponseSMS.lstSmsEmailDetails[0].Template);
                    templat.Replace(CUSTOMER_NAME, CustomerName);
                    templat.Replace(E_VOUCHER_NAME, eVoucherName);
                    templat.Replace(CARD_NUMBER, eVoucherNumber);
                    if (PinType == SAVED_PIN)
                        templat.Replace(PIN, Pin);
                    StringBuilder data = new StringBuilder();
                    data.Append(UIConstants.WORKING_KEY + workingkey);
                    if (!string.IsNullOrEmpty(Mobile))
                    {
                        data.Append(UIConstants.SMS_TO + Mobile);
                        data.Append(UIConstants.SMS_SENDER + smssender);
                        data.Append(UIConstants.SMS_MESSAGE + templat);
                    }
                    UICommon.Send_Sms(Convert.ToString(data), "");
                }
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GE_SMSMail_RecepientMobileApp() " + ex.Message);
            }


            return result;
        }

        public bool GE_SMSMail_CardOwnerMobileApp(string EmailId, string Mobile, string CustomerName, string PinType, string eVoucherName, string eVoucherNumber, string ReceipentName, string MerchantName, string CurrentUserName, int CardIssueID, string Title)
        {
            bool result = default(bool);
            try
            {
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                string Pin = string.Empty;
                if (PinType == SAVED_PIN)
                    Pin = GetCardPin(CardIssueID);

                SmsEmail ObjSmsEmail = new SmsEmail();


                if (PinType == SAVED_PIN)
                    ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.GiftAneVoucherWithPIN_ToGifter);
                else
                    ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.GiftaneVoucherwithoutPINOrwithOTP_ToGifter);
                ObjSmsEmailRetrieveRequest.Type = UIConstants.EMAIL;
                ObjSmsEmailRetrieveRequest.UserName = MerchantName;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = EmailId;
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse = mobileAppBO.GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);
                if (ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count > 0)
                {
                    string Subject = ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[0].Subject;
                    StringBuilder MailBody = new StringBuilder();
                    MailBody.Append(ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[0].Template);
                    MailBody.Replace(CUSTOMER_NAME, (Title + CurrentUserName));
                    MailBody.Replace(RECIPIENT_NAME, ReceipentName);
                    MailBody.Replace(E_VOUCHER_NAME, eVoucherName);
                    MailBody.Replace(CARD_NUMBER, eVoucherNumber);
                    if (PinType == SAVED_PIN)
                        MailBody.Replace(PIN, Pin);


                    //Get SMTP details from DB.==============================================================================
                    List<string> smtpdetailslst = SMTP.GetSmtp(MerchantName);
                    string SMTPAddress = smtpdetailslst[0].ToString();
                    string SMTPUserName = smtpdetailslst[1].ToString();
                    string SMTPPassword = smtpdetailslst[2].ToString();
                    string FromEmail = smtpdetailslst[3].ToString();
                    //================================================
                    Thread email = new Thread(delegate ()
                    {
                        MailUtil.SendMail(EmailId, Subject, MailBody, "", SMTPAddress, SMTPUserName, SMTPPassword, FromEmail);
                    });
                    email.Start();
                    email.IsBackground = true;
                }

                //SMS

                ObjSmsEmail = new SmsEmail();
                mobileAppBO = new MobileAppBO();
                ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.GiftAneVoucherWithPIN_ToGifter);
                ObjSmsEmailRetrieveRequest.Type = UIConstants.SMS;
                ObjSmsEmailRetrieveRequest.UserName = MerchantName;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = Mobile;
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponseSMS = mobileAppBO.GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);
                if (ObjSmsEmailRetrieveResponseSMS.lstSmsEmailDetails.Count > 0)
                {
                    StringBuilder templat = new StringBuilder();
                    templat.Append(ObjSmsEmailRetrieveResponseSMS.lstSmsEmailDetails[0].Template);
                    templat.Replace(CUSTOMER_NAME, CurrentUserName);
                    templat.Replace(E_VOUCHER_NAME, eVoucherName);
                    templat.Replace(CARD_NUMBER, eVoucherNumber);

                    StringBuilder data = new StringBuilder();
                    data.Append(UIConstants.WORKING_KEY + workingkey);
                    if (!string.IsNullOrEmpty(Mobile))
                    {
                        data.Append(UIConstants.SMS_TO + Mobile);
                        data.Append(UIConstants.SMS_SENDER + smssender);
                        data.Append(UIConstants.SMS_MESSAGE + templat);
                    }
                    UICommon.Send_Sms(Convert.ToString(data), "");
                }
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GE_SMSMail_CardOwnerMobileApp() " + ex.Message);
            }

            return result;
        }
        //End : Evoucher Gift

        //Start : PointBalance Gift
        public bool EnCh_SMSMail_RecepientMobileApp(string EmailId, string Mobile, string CustomerName, string MerchantName, string CurrentUserName, int CurrentLoginId, string LoyaltyId, string Title)
        {
            bool result = default(bool);
            try
            {
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();

                SmsEmail ObjSmsEmail = new SmsEmail();


                ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.CustomerGiftingPoints_ToRecipient);
                ObjSmsEmailRetrieveRequest.Type = UIConstants.EMAIL;
                ObjSmsEmailRetrieveRequest.UserName = MerchantName;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = EmailId;
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse = mobileAppBO.GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);
                if (ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count > 0)
                {
                    string Subject = ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[0].Subject;
                    List<CustomerPointBalance> lstPoints = GetPointBalance(CurrentLoginId, LoyaltyId);
                    SENDER_POINTS = lstPoints[0].SenderPointBalance.ToString();
                    RECEIPENT_POINTS = lstPoints[0].ReceiverPointBalance.ToString();
                    TRANSFER_POINTS = lstPoints[0].TransferPoints.ToString();
                    StringBuilder MailBody = new StringBuilder();
                    MailBody.Append(ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[0].Template);
                    MailBody.Replace(CUSTOMER_NAME, (Title + CustomerName));
                    MailBody.Replace(GIFTERS_NAME, CurrentUserName);
                    MailBody.Replace(TRANSWER_POINTS, TRANSFER_POINTS);
                    MailBody.Replace(POINTS_IN_ACCOUNT, RECEIPENT_POINTS);
                    //Get SMTP details from DB.==============================================================================
                    List<string> smtpdetailslst = SMTP.GetSmtp(MerchantName);
                    string SMTPAddress = smtpdetailslst[0].ToString();
                    string SMTPUserName = smtpdetailslst[1].ToString();
                    string SMTPPassword = smtpdetailslst[2].ToString();
                    string FromEmail = smtpdetailslst[3].ToString();
                    //================================================

                    Thread email = new Thread(delegate ()
                    {
                        MailUtil.SendMail(EmailId, Subject, MailBody, "", SMTPAddress, SMTPUserName, SMTPPassword, FromEmail);
                    });
                    email.Start();
                    email.IsBackground = true;
                }
                //SMS

                ObjSmsEmail = new SmsEmail();
                mobileAppBO = new MobileAppBO();
                ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.CustomerGiftingPoints_ToRecipient);
                ObjSmsEmailRetrieveRequest.Type = UIConstants.SMS;
                ObjSmsEmailRetrieveRequest.UserName = MerchantName;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = Mobile;
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponseSMS = mobileAppBO.GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);
                if (ObjSmsEmailRetrieveResponseSMS.lstSmsEmailDetails.Count > 0)
                {
                    StringBuilder templat = new StringBuilder();
                    templat.Append(ObjSmsEmailRetrieveResponseSMS.lstSmsEmailDetails[0].Template);
                    templat.Replace(CUSTOMER_NAME, CustomerName);
                    templat.Replace(TRANSWER_POINTS, TRANSFER_POINTS);
                    templat.Replace(BALANCE, RECEIPENT_POINTS);

                    StringBuilder data = new StringBuilder();
                    data.Append(UIConstants.WORKING_KEY + workingkey);
                    if (!string.IsNullOrEmpty(Mobile))
                    {
                        data.Append(UIConstants.SMS_TO + Mobile);
                        data.Append(UIConstants.SMS_SENDER + smssender);
                        data.Append(UIConstants.SMS_MESSAGE + templat);
                    }
                    UICommon.Send_Sms(Convert.ToString(data), "");
                }
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " EnCh_SMSMail_RecepientMobileApp() " + ex.Message);
            }
            return result;
        }

        public bool EnCh_SMSMail_CardOwnerMobileApp(string EmailId, string Mobile, string CustomerName, string RecipentName, string MerchantName, string CurrentUserName, int CurrentLoginId, string LoyaltyId, string Title)
        {
            bool result = default(bool);
            try
            {
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();

                SmsEmail ObjSmsEmail = new SmsEmail();


                ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.CustomerGiftingPoints_ToGifter);
                ObjSmsEmailRetrieveRequest.Type = UIConstants.EMAIL;
                ObjSmsEmailRetrieveRequest.UserName = MerchantName;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = EmailId;
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse = mobileAppBO.GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);
                if (ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count > 0)
                {
                    string Subject = ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[0].Subject;

                    List<CustomerPointBalance> lstPoints = GetPointBalance(CurrentLoginId, LoyaltyId);
                    SENDER_POINTS = lstPoints[0].SenderPointBalance.ToString();
                    RECEIPENT_POINTS = lstPoints[0].ReceiverPointBalance.ToString();
                    TRANSFER_POINTS = lstPoints[0].TransferPoints.ToString();

                    StringBuilder MailBody = new StringBuilder();
                    MailBody.Append(ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[0].Template);
                    MailBody.Replace(CUSTOMER_NAME, (Title + CurrentUserName));
                    MailBody.Replace(RECIPIENT_NAME, RecipentName);
                    MailBody.Replace(TRANSWER_POINTS, TRANSFER_POINTS);
                    MailBody.Replace(POINTS_IN_ACCOUNT, SENDER_POINTS);
                    //Get SMTP details from DB.==============================================================================
                    List<string> smtpdetailslst = SMTP.GetSmtp(MerchantName);
                    string SMTPAddress = smtpdetailslst[0].ToString();
                    string SMTPUserName = smtpdetailslst[1].ToString();
                    string SMTPPassword = smtpdetailslst[2].ToString();
                    string FromEmail = smtpdetailslst[3].ToString();
                    //================================================

                    Thread email = new Thread(delegate ()
                    {
                        MailUtil.SendMail(EmailId, Subject, MailBody, "", SMTPAddress, SMTPUserName, SMTPPassword, FromEmail);
                    });
                    email.Start();
                    email.IsBackground = true;
                }

                //SMS
                ObjSmsEmail = new SmsEmail();
                mobileAppBO = new MobileAppBO();
                ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.CustomerGiftingPoints_ToGifter);
                ObjSmsEmailRetrieveRequest.Type = UIConstants.SMS;
                ObjSmsEmailRetrieveRequest.UserName = MerchantName;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = Mobile;
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponseSMS = mobileAppBO.GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);
                if (ObjSmsEmailRetrieveResponseSMS.lstSmsEmailDetails.Count > 0)
                {
                    StringBuilder templat = new StringBuilder();
                    templat.Append(ObjSmsEmailRetrieveResponseSMS.lstSmsEmailDetails[0].Template);
                    templat.Replace(CUSTOMER_NAME, CurrentUserName);
                    templat.Replace(TRANSWER_POINTS, TRANSFER_POINTS);
                    //templat.Replace(POINTS_IN_ACCOUNT, SENDER_POINTS);
                    templat.Replace(BALANCE, SENDER_POINTS);
                    StringBuilder data = new StringBuilder();
                    data.Append(UIConstants.WORKING_KEY + workingkey);
                    if (!string.IsNullOrEmpty(Mobile))
                    {
                        data.Append(UIConstants.SMS_TO + Mobile);
                        data.Append(UIConstants.SMS_SENDER + smssender);
                        data.Append(UIConstants.SMS_MESSAGE + templat);
                    }
                    UICommon.Send_Sms(Convert.ToString(data), "");
                }
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " EnCh_SMSMail_CardOwnerMobileApp() " + ex.Message);
            }

            return result;
        }
        //End : PointBalance Gift

        private List<CustomerPointBalance> GetPointBalance(int ActorId, string LoyaltyId)
        {
            List<CustomerPointBalance> lstpoints = new List<CustomerPointBalance>();

            try
            {

                GiftCardIssueRetrieveRequest ObjGiftCardIssueRetrieveRequest = new GiftCardIssueRetrieveRequest();
                ObjGiftCardIssueRetrieveRequest.ActorId = ActorId;
                ObjGiftCardIssueRetrieveRequest.LoyaltyId = LoyaltyId;
                GiftCardIssueRetrieveResponse ObjGiftCardIssueRetrieveResponse = mobileAppBO.GetCustomerPointBalanceMobileApp(ObjGiftCardIssueRetrieveRequest);
                if (ObjGiftCardIssueRetrieveResponse.lstPointbalance.Count > 0)
                    lstpoints = ObjGiftCardIssueRetrieveResponse.lstPointbalance;
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetPointBalance() " + ex.Message);
            }
            return lstpoints;
        }

        private string GetCardPin(int CardId)
        {
            string Pin = string.Empty;
            try
            {

                GiftCardIssueRetrieveRequest ObjGiftCardIssueRetrieveRequest = new GiftCardIssueRetrieveRequest();
                ObjGiftCardIssueRetrieveRequest.CardIssueId = CardId;
                GiftCardIssueRetrieveResponse ObjGiftCardIssueRetrieveResponse = mobileAppBO.GetCardPinBasedOnIssueIdMobileApp(ObjGiftCardIssueRetrieveRequest);
                if (ObjGiftCardIssueRetrieveResponse.ReturnMessage != null)
                {
                    Pin = UICommon.Decrypt(ObjGiftCardIssueRetrieveResponse.ReturnMessage);
                }
            }
            catch (Exception ex)
            {

                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetCardPin() " + ex.Message);
            }

            return Pin;
        }

        public bool forgotPasswordMobileApp(string UserName, string MerchantUserName = null)
        {
            bool result = default(bool);
            try
            {
                UserRetrieveRequest_Loyalty userRetrieveRequest = new UserRetrieveRequest_Loyalty();
                userRetrieveRequest.UserName = PAGE_PREFIX + ":" + UserName;
                userRetrieveRequest.Password = string.Empty;
                userRetrieveRequest.UserId = -1;
                userRetrieveRequest.ActionType = CHECK_LOLALTYID_EXISTS;
                string RandomPin_OTP = UICommon.GetRandomNumber();
                string Encrypted_OTP = Security.EncryptPassword(RandomPin_OTP);
                userRetrieveRequest.Password = Encrypted_OTP;
                userRetrieveRequest.MerchantUserName = MerchantUserName;
                UserLoginRetrieveResponse userRetrieveResponse = CheckIsAuthenticatedMobileApp(userRetrieveRequest);
                if (userRetrieveResponse != null)
                {
                    if (userRetrieveResponse.UserList.Count > 0)
                    {
                        if (userRetrieveResponse.UserList[0].Result == 1)
                        {
                            string ToName = userRetrieveResponse.UserList[0].CommonUserName;
                            string ToNumber = userRetrieveResponse.UserList[0].CommonUserMobile;
                            string UserTyp = userRetrieveResponse.UserList[0].UserType;
                            string Uname = userRetrieveResponse.UserList[0].UserName;
                            string Mail = userRetrieveResponse.UserList[0].Email;

                            /*For sending the Forgot Pin through SMS */
                            SendForgotPIN(ToName, ToNumber, RandomPin_OTP, UserTyp, Uname);

                            /*For sending the Forgot Pin through EMAIL */
                            if (UserTyp == UIConstants.CUSTOMER_TYPE || UserTyp == UIConstants.USER_TYPE)
                            {
                                string Prefix = userRetrieveResponse.UserList[0].Prefix;
                                ToName = Prefix + " " + ToName;
                            }
                            else
                            {
                                ToName = userRetrieveResponse.UserList[0].CommonUserName;
                            }
                            SendForgotPINMail(ToName, Mail, RandomPin_OTP, Uname);
                            result = true;
                        }
                        else
                        {
                            result = false;
                        }
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " forgotPasswordMobileApp() " + ex.Message);
            }
            return result;
        }

        public void SendForgotPIN(string CustName, string Mobile, string PinNumber, string UsTyp, string Uname)
        {
            try
            {
                mobileAppBO = new MobileAppBO();
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.ForgotPasswordPIN);
                ObjSmsEmailRetrieveRequest.Type = UIConstants.SMS;
                ObjSmsEmailRetrieveRequest.UserName = Uname;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = Mobile;
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse = mobileAppBO.GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);
                if ((ObjSmsEmailRetrieveResponse != null) && (ObjSmsEmailRetrieveResponse.lstSmsEmailDetails != null) && (ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count > 0))
                {

                    for (int i = 0; i < ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count; i++)
                    {
                        List<AlertTemplateDynamicContent> DynamicTemplate = new List<AlertTemplateDynamicContent>()
                            {
                            new AlertTemplateDynamicContent(TO_NAME, CustName),
                            new AlertTemplateDynamicContent(PIN, PinNumber)
                            };

                        //Sending SMS
                        AlertUtiltityParameters alertUtiltityParametersSMS = new AlertUtiltityParameters
                                      (ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i].Type, Mobile, DynamicTemplate, ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i]);
                        if (DOMAIN == UIConstants.MILLER_CLUB)
                            alertUtiltityParametersSMS.VendorMethodName = SMSVendorMethodEnum.Skylinesms;
                        else if (DOMAIN == UIConstants.REDINGTON)
                            alertUtiltityParametersSMS.VendorMethodName = SMSVendorMethodEnum.SmsCountry;
                        alertUtiltityParametersSMS.IsGetRequest = true;
                        var SMSResult = SendAlertUtility.SendAlert(alertUtiltityParametersSMS);
                    }
                }

            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " SendForgotPIN() " + ex.Message);
            }
        }

        private void SendForgotPINMail(string CustName, string Mail, string PinNumber, string Uname)
        {
            try
            {
                mobileAppBO = new MobileAppBO();
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.ForgotPasswordPIN);
                ObjSmsEmailRetrieveRequest.Type = UIConstants.EMAIL;
                ObjSmsEmailRetrieveRequest.UserName = Uname;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = Mail;
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse = mobileAppBO.GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);
                if ((ObjSmsEmailRetrieveResponse != null) && (ObjSmsEmailRetrieveResponse.lstSmsEmailDetails != null) && (ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count > 0))
                {
                    for (int i = 0; i < ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count; i++)
                    {

                        List<AlertTemplateDynamicContent> DynamicTemplate = new List<AlertTemplateDynamicContent>()
                            {
                            new AlertTemplateDynamicContent(TO_NAME, CustName),
                            new AlertTemplateDynamicContent(PIN, PinNumber),
                            new AlertTemplateDynamicContent("[UserName]", Uname)
                            };

                        //Sending Mail
                        AlertUtiltityParameters alertUtiltityParametersEmail = new AlertUtiltityParameters
                                      (UIConstants.EMAIL, Mail, DynamicTemplate, ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i]);
                        alertUtiltityParametersEmail.MailHeaderText = UIConstants.OTHERS_HEADER;
                        var EmailResult = SendAlertUtility.SendAlert(alertUtiltityParametersEmail);
                    }
                }
            }
            catch (Exception ex)
            {

                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " SendForgotPINMail() " + ex.Message);
            }

        }

        public bool sendCustomerFeedback(string MemberId, string MemberEmail, string Mobile, string MemberName)
        {
            bool result = default(bool);
            try
            {

                string EnquiryEmail = ConfigurationManager.AppSettings[ENQUIRY_EMAIL];
                string EnquirySubject = ConfigurationManager.AppSettings[ENQUIRY_SUBJECT];

                StringBuilder objStringBuilder = new StringBuilder();
                objStringBuilder.Append("<html><head><style type='text/css'></style></head>");
                objStringBuilder.Append("<body><table border='0' cellpadding='6' cellspacing='0' width='100%' style='font-family:calibri,arial; font-size:14px;'><tbody>");
                objStringBuilder.Append("<tr><td class='style1'><b>Hi Team,</b></td></tr>");
                objStringBuilder.Append("<tr><td class='style2' style='line-height:1.5em;' >Member # :" + MemberId + ".");
                objStringBuilder.Append("<tr><td class='style2' style='line-height:0.5em;' >Name : " + MemberName + ".");
                objStringBuilder.Append("<tr><td class='style2' style='line-height:0.5em;' >Email : " + MemberEmail + ".");
                objStringBuilder.Append("<tr><td class='style2' style='line-height:0.5em;' >Mobile : " + Mobile + ".");
                objStringBuilder.Append("<tr><td class='style2'><br/> Regards,");
                objStringBuilder.Append("<br/>" + MemberName);
                objStringBuilder.Append("</tbody></table></body></html>");

                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse = new SmsEmailRetrieveResponse();
                ObjSmsEmailRetrieveResponse.lstSmsEmailDetails = new List<SmsEmail>();
                for (int i = 0; i < ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count; i++)
                {
                    List<AlertTemplateDynamicContent> DynamicTemplate = new List<AlertTemplateDynamicContent>();
                    SmsEmail objSmsEmail = new SmsEmail();
                    objSmsEmail.Template = objStringBuilder.ToString();
                    objSmsEmail.Subject = EnquirySubject;

                    ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Add(objSmsEmail);

                    AlertUtiltityParameters alertUtiltityParametersEmail = new AlertUtiltityParameters
                                      (UIConstants.EMAIL, EnquiryEmail, DynamicTemplate, ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i]);
                    alertUtiltityParametersEmail.MailHeaderText = UIConstants.OTHERS_HEADER;
                    var EmailResult = SendAlertUtility.SendAlert(alertUtiltityParametersEmail);
                }

            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " sendCustomerFeedback() " + ex.Message);
            }

            return result;
        }

        #endregion SMS & EMAIL

        public bool CheckEmailMobileExistsMobileApp(LocationSaveRequest ObjLocationSaveRequest)
        {
            bool returnValue = default(bool);
            try
            {

                LocationSaveResponse ObjLocationSaveResponse = mobileAppBO.CheckEmailMobileExistsMobileApp(ObjLocationSaveRequest);
                if (ObjLocationSaveResponse.ReturnValue == 1)
                    returnValue = false;
                else
                    returnValue = true;
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " CheckEmailMobileExistsMobileApp() " + ex.Message);
            }
            return returnValue;
        }

        #region Dhinakaran

        #region Customer Referal
        public CustomerSaveResponse SaveCustomerRefrenceDetailsMobileApp(CustomerSaveRequest objCustomerSaveRequest)
        {
            CustomerSaveResponse objCustomerSaveResponse = null;
            try
            {

                string point = string.Empty, CurentPoint = string.Empty;
                //for Referal
                string ReferalName = objCustomerSaveRequest.ObjCustomerBasicInfo.Title + " " + objCustomerSaveRequest.ObjCustomerBasicInfo.FirstName;
                string ReferalEmail = objCustomerSaveRequest.ObjCustomerBasicInfo.Email;
                string ReferealMobile = objCustomerSaveRequest.ObjCustomerBasicInfo.Mobile;

                //For Referee
                string RefereeName = objCustomerSaveRequest.ObjCustomer.Title + " " + objCustomerSaveRequest.ObjCustomer.FirstName;
                string RefereeEmail = objCustomerSaveRequest.ObjCustomer.Email;
                string RefereeMobile = objCustomerSaveRequest.ObjCustomer.Mobile;
                string Username = objCustomerSaveRequest.ObjCustomer.UserName;


                objCustomerSaveResponse = mobileAppBO.SaveCustomerRefrenceDetailsMobileApp(objCustomerSaveRequest);
                if (objCustomerSaveResponse != null)
                {
                    if (objCustomerSaveResponse.ReturnMessage.Split('~')[0].ToString() == "1")
                    {
                        if (objCustomerSaveResponse.ReturnMessage.Split('~').Length > 1)
                        {
                            point = objCustomerSaveResponse.ReturnMessage.Split('~')[1].ToString();
                            CurentPoint = objCustomerSaveResponse.ReturnMessage.Split('~')[2].ToString();
                        }

                        //for Referal
                        SendToReferralMail(ReferalName, ReferalEmail, RefereeName, Username);
                        //For Referee
                        SendToRefereeMail(RefereeName, RefereeEmail, ReferalName, Username, point, CurentPoint);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "SaveCustomerRefrenceDetailsMobileApp() : " + ex.Message);
            }

            return objCustomerSaveResponse;
        }


        #region Email and Sms

        //For Referee to Referral
        private void SendToReferralMail(string ToName, string ToMail, string Referername, string Uname)
        {
            try
            {
                SmsEmail ObjSmsEmail = new SmsEmail();

                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.ReferredDetailOfReferral);
                ObjSmsEmailRetrieveRequest.Type = UIConstants.EMAIL;
                ObjSmsEmailRetrieveRequest.UserName = Uname;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = ToMail;
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse = mobileAppBO.GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);
                if ((ObjSmsEmailRetrieveResponse != null) && (ObjSmsEmailRetrieveResponse.lstSmsEmailDetails != null) && (ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count > 0))
                {


                    for (int i = 0; i < ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count; i++)
                    {

                        List<AlertTemplateDynamicContent> DynamicTemplate = new List<AlertTemplateDynamicContent>()
                            {
                            new AlertTemplateDynamicContent(TO_NAME, ToName),
                            new AlertTemplateDynamicContent(RefereeName, Referername)
                            };

                        AlertUtiltityParameters alertUtiltityParameters = new AlertUtiltityParameters
                     (UIConstants.EMAIL, ToMail, DynamicTemplate, ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i]);

                        List<string> smtpdetailslst = GetSmtp(Uname);
                        alertUtiltityParameters.MailHeaderText = UIConstants.OTHERS_HEADER;
                        alertUtiltityParameters.ServerDetails = new Dictionary<string, string>
                {
                    {smtpdetailslst[0], "SMTPAddress"},
                    {smtpdetailslst[1], "SMTPUserName"},
                    {smtpdetailslst[2], "SMTPPassword"},
                    {smtpdetailslst[3], "FromEmail"}
                };

                        alertUtiltityParameters.ShouldUseThreading = false;
                        var AlertResult = SendAlertUtility.SendAlert(alertUtiltityParameters);
                    }
                }
            }
            catch (Exception ex)
            {

                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " SendToReferralMail() " + ex.Message);
            }

        }

        //For Referral to Referee
        private void SendToRefereeMail(string ToName, string ToMail, string Referername, string Uname, string CreditPoint, string Curenpoint)
        {
            try
            {
                SmsEmail ObjSmsEmail = new SmsEmail();

                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.ReferredDetailOfReferee);
                ObjSmsEmailRetrieveRequest.Type = UIConstants.EMAIL;
                ObjSmsEmailRetrieveRequest.UserName = Uname;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = ToMail;
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse = mobileAppBO.GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);
                if ((ObjSmsEmailRetrieveResponse != null) && (ObjSmsEmailRetrieveResponse.lstSmsEmailDetails != null) && (ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count > 0))
                {

                    for (int i = 0; i < ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count; i++)
                    {
                        string Subject = ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[0].Subject;
                        StringBuilder MailBody = new StringBuilder();
                        MailBody.Append(ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[0].Template);
                        MailBody.Replace(TO_NAME, ToName);
                        MailBody.Replace(ReferralName, Referername);
                        MailBody.Replace(NoOfPoints, CreditPoint);
                        MailBody.Replace(CurrentPoint, Curenpoint);

                        List<AlertTemplateDynamicContent> DynamicTemplate = new List<AlertTemplateDynamicContent>()
                            {
                            new AlertTemplateDynamicContent(TO_NAME, ToName),
                            new AlertTemplateDynamicContent(ReferralName, Referername),
                             new AlertTemplateDynamicContent(NoOfPoints, CreditPoint) ,
                             new AlertTemplateDynamicContent(CurrentPoint, Curenpoint)
                            };

                        AlertUtiltityParameters alertUtiltityParameters = new AlertUtiltityParameters
                     (UIConstants.EMAIL, ToMail, DynamicTemplate, ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i]);

                        List<string> smtpdetailslst = GetSmtp(Uname);

                        alertUtiltityParameters.ServerDetails = new Dictionary<string, string>
                {
                    {smtpdetailslst[0], "SMTPAddress"},
                    {smtpdetailslst[1], "SMTPUserName"},
                    {smtpdetailslst[2], "SMTPPassword"},
                    {smtpdetailslst[3], "FromEmail"}
                };

                        alertUtiltityParameters.ShouldUseThreading = false;
                        alertUtiltityParameters.MailHeaderText = UIConstants.OTHERS_HEADER;
                        var AlertResult = SendAlertUtility.SendAlert(alertUtiltityParameters);
                    }
                }
            }
            catch (Exception ex)
            {

                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " SendToRefereeMail() " + ex.Message);
            }

        }

        #endregion

        #endregion

        #region Get Location Detail
        public LocationRetrieveResponse GetLocationDetailsMobileApp(LocationRetrieveRequest ObjLocationRetrieveRequest)
        {
            LocationRetrieveResponse ObjLocationRetrieveResponse = null;
            try
            {

                ObjLocationRetrieveResponse = mobileAppBO.GetLocationDetailsMobileApp(ObjLocationRetrieveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetLocationDetailsMobileApp : " + ex.Message);
            }
            return ObjLocationRetrieveResponse;
        }
        #endregion

        #region Save customer Book appointment
        public CustomerSaveResponse SaveCustomerBookAppointmentMobileApp(CustomerSaveRequest ObjCustomerSaveRequest)
        {

            CustomerSaveResponse objCustomerSaveResponse = null;
            try
            {
                objCustomerSaveResponse = mobileAppBO.SaveCustomerBookAppointmentMobileApp(ObjCustomerSaveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "SaveCustomerBookAppointmentMobileApp : " + ex.Message);
            }
            return objCustomerSaveResponse;

        }
        #endregion

        #endregion

        #region Promotions

        public PromotionRetrieveResponse GetPromotionDetailsMobileApp(PromotionRetrieveRequest ObjPromotionRetrieveRequest)
        {
            PromotionRetrieveResponse ObjPromotionRetrieveResponse = null;
            try
            {

                ObjPromotionRetrieveResponse = mobileAppBO.GetPromotionDetailsMobileApp(ObjPromotionRetrieveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "GetPromotionDetails : " + ex.Message);
            }
            return ObjPromotionRetrieveResponse;
        }

        public PromotionRetrieveResponse GetCustomerPromotionDetailsByPromotionID(PromotionRetrieveRequest ObjPromotionRetrieveRequest)
        {
            PromotionRetrieveResponse objPromotionRetrieveResponse = null;
            try
            {

                objPromotionRetrieveResponse = mobileAppBO.GetCustomerPromotionDetailsByPromotionID(ObjPromotionRetrieveRequest);

                if (objPromotionRetrieveResponse != null)
                {
                    if (objPromotionRetrieveResponse.LstPromotionJsonList != null && objPromotionRetrieveResponse.LstPromotionJsonList.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(objPromotionRetrieveResponse.LstPromotionJsonList[0].ProLongDesc))
                        {
                            StringBuilder LongDescription = new StringBuilder();
                            LongDescription.Append(objPromotionRetrieveResponse.LstPromotionJsonList[0].ProLongDesc);

                            LongDescription.Replace(TITLE, objPromotionRetrieveResponse.LstPromotionJsonList[0].Title);
                            LongDescription.Replace(FIRST_NAME, objPromotionRetrieveResponse.LstPromotionJsonList[0].FirstName);
                            LongDescription.Replace(LAST_NAME, objPromotionRetrieveResponse.LstPromotionJsonList[0].LastName);
                            LongDescription.Replace(NAME, objPromotionRetrieveResponse.LstPromotionJsonList[0].FullName);
                            LongDescription.Replace(MEMBER_ID, objPromotionRetrieveResponse.LstPromotionJsonList[0].MemberId);
                            LongDescription.Replace(POINT_BALANCE, objPromotionRetrieveResponse.LstPromotionJsonList[0].PointBalance.ToString());

                            objPromotionRetrieveResponse.LstPromotionJsonList[0].ProLongDesc = LongDescription.ToString();
                        }

                    }
                }
            }
            catch (Exception ex)
            {

                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "GetCustomerPromotionDetailsByPromotionID()" + ex.Message);
            }
            return objPromotionRetrieveResponse;
        }

        public PromotionDetailsSaveResponse SaveCustomerPromotionDetails(PromotionDetailsSaveRequest ObjPromotionDetailsSaveRequest)
        {
            PromotionDetailsSaveResponse objPromotionDetailsSaveResponse = null;
            try
            {


                if (ObjPromotionDetailsSaveRequest.ActionType == (int)Enumeration_Loyalty.ActionType.Save_Cusromer_Promotion_Claim_Details)
                {
                    string ClaimID = UICommon.GetRandomNumber();
                    ObjPromotionDetailsSaveRequest.ClaimId = ClaimID;
                }
                else
                    ObjPromotionDetailsSaveRequest.ClaimId = string.Empty;
                objPromotionDetailsSaveResponse = mobileAppBO.SaveCustomerPromotionDetails(ObjPromotionDetailsSaveRequest);
                if (objPromotionDetailsSaveResponse != null)
                {
                    string returnMsg = objPromotionDetailsSaveResponse.ReturnMessage;
                    if (ObjPromotionDetailsSaveRequest.ActionType == (int)Enumeration_Loyalty.ActionType.Save_Cusromer_Promotion_Claim_Details)
                    {
                        switch (returnMsg.Split('~')[0])
                        {
                            case "1":
                                string PromotionType = returnMsg.Split('~')[2];

                                if (PromotionType == UIConstants.VALUE_TWO)
                                    objPromotionDetailsSaveResponse.ReturnMessage = "SMS with PIN No. has been sent to your registered mobile no, please quote this PIN no. at the time of participating.";
                                else
                                    objPromotionDetailsSaveResponse.ReturnMessage = "SMS with PIN No. has been sent to your registered mobile no, please quote this PIN no. at the time of availing this offer.";

                                string Name, Mobile, PromotionName, PClaimId, LoyaltyId;
                                Name = ObjPromotionDetailsSaveRequest.CustomerName;
                                Mobile = ObjPromotionDetailsSaveRequest.Mobile;
                                PromotionName = ObjPromotionDetailsSaveRequest.PromotionName;
                                PClaimId = returnMsg.Split('~')[1];
                                LoyaltyId = ObjPromotionDetailsSaveRequest.LoyaltyId;
                                SendClaimCodeSms(Name, Mobile, PClaimId, PromotionName, LoyaltyId);
                                break;
                            case "-1":
                                objPromotionDetailsSaveResponse.ReturnMessage = "Claim option not available for this Promotion.";
                                break;
                            case "-2":
                                objPromotionDetailsSaveResponse.ReturnMessage = "Already Claim is done with this promotion.";
                                break;
                            case "-3":
                                objPromotionDetailsSaveResponse.ReturnMessage = "Claim count exceeded.";
                                break;
                            case "-4":
                                objPromotionDetailsSaveResponse.ReturnMessage = "Claim details saved failed.";
                                break;
                            case "-5":
                                objPromotionDetailsSaveResponse.ReturnMessage = "Claim option expired for this promotion.";
                                break;
                        }
                    }
                    else if (ObjPromotionDetailsSaveRequest.ActionType == (int)Enumeration_Loyalty.ActionType.Save_Customer_Promotion_Reserve_Details)
                    {
                        switch (returnMsg)
                        {
                            case "1":
                                if ((ObjPromotionDetailsSaveRequest.IsReserved == true) && (ObjPromotionDetailsSaveRequest.IsUnReserved == false))
                                {
                                    objPromotionDetailsSaveResponse.ReturnMessage = "Promotion reserved  successfully.";
                                }
                                else if ((ObjPromotionDetailsSaveRequest.IsReserved == false) && (ObjPromotionDetailsSaveRequest.IsUnReserved == true))
                                {
                                    objPromotionDetailsSaveResponse.ReturnMessage = "Promotion Un-reserved successfully.";
                                }

                                break;
                            case "-1":
                                objPromotionDetailsSaveResponse.ReturnMessage = "Reserve option not available for this Promotion.";
                                break;
                            case "-2":
                                objPromotionDetailsSaveResponse.ReturnMessage = "Already done with this promotion.";
                                break;
                            case "-3":
                                objPromotionDetailsSaveResponse.ReturnMessage = "Reserve option expired for this promotion.";
                                break;
                            case "-4":
                                objPromotionDetailsSaveResponse.ReturnMessage = "Maximium reserve count exceeded for this promotion.";
                                break;
                            case "-5":
                                objPromotionDetailsSaveResponse.ReturnMessage = "Maximum claims for this promotion has been done. Try next time.";
                                break;
                        }
                    }
                    else if (ObjPromotionDetailsSaveRequest.ActionType == (int)Enumeration_Loyalty.ActionType.Save_Customer_Promotion_Comment_Details)
                    {
                        switch (returnMsg)
                        {
                            case "1":
                                objPromotionDetailsSaveResponse.ReturnMessage = "Comments Saved  successfully.";
                                break;
                            case "-1":
                                objPromotionDetailsSaveResponse.ReturnMessage = "Comment option not available for this Promotion.";
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "SaveCustomerPromotionDetails()" + ex.Message);
            }
            return objPromotionDetailsSaveResponse;
        }

        #region Send Claim Code By SMS
        private void SendClaimCodeSms(string CustName, string Mobile, string ClaimCode, string PromotionName, string LoyaltyId)
        {
            try
            {
                SmsEmail ObjSmsEmail = new SmsEmail();

                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.PromotionClaimIDGeneration);
                ObjSmsEmailRetrieveRequest.Type = UIConstants.SMS;
                ObjSmsEmailRetrieveRequest.UserName = LoyaltyId;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = Mobile;
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse = mobileAppBO.GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);

                for (int i = 0; i < ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count; i++)
                {
                    List<AlertTemplateDynamicContent> DynamicTemplate = new List<AlertTemplateDynamicContent>(){
                new AlertTemplateDynamicContent (ClientName,CustName),
                new AlertTemplateDynamicContent (CLAIM_ID,ClaimCode),
                new AlertTemplateDynamicContent(PROMO_NAME,PromotionName)};

                    AlertUtiltityParameters alertUtiltityParameters = new AlertUtiltityParameters
                   (ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i].Type, Mobile, DynamicTemplate, ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i]);
                    alertUtiltityParameters.MailHeaderText = UIConstants.PROMOTION_HEADER;
                    var Result = SendAlertUtility.SendAlert(alertUtiltityParameters);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " SendClaimCodeSms() " + ex.Message);
            }
        }
        #endregion Send Claim Code By SMS


        #endregion Promotions

        #region Recharge

        public RechargeSaveTransactionRespnse SaveMobileRechargeTransactionDetails(RechargeSaveTransactionRequest objRechargeSaveTransactionRequest)
        {

            RechargeSaveTransactionRespnse objRechargeSaveTransactionRespnse = null;
            string RechargeResult = string.Empty;
            try
            {
                //Description :
                //RECHARGE_STATUS= 0 ----------Default
                //RECHARGE_STATUS= 1 ----------Success
                //RECHARGE_STATUS= 2 ----------Pending
                //RECHARGE_STATUS= 3 ----------Failure

                int rechargeStatus = 0;

                string rechargeResult = string.Empty;
                string APIId, APIPassword, APIPin = string.Empty;


                if (objRechargeSaveTransactionRequest != null)
                {
                    Mobile = objRechargeSaveTransactionRequest.Mobile;
                    OperatorCode = objRechargeSaveTransactionRequest.OperatorCode;
                    CircleId = objRechargeSaveTransactionRequest.CircleId;
                    Amount = objRechargeSaveTransactionRequest.Amount;
                }
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                var random = new Random();
                string rdmNumber = new string(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray());

                int rVal = 0;
                objRechargeSaveTransactionRequest.ApplicationTrnxId = rdmNumber;
                objRechargeSaveTransactionRequest.TrxnID = rVal;
                objRechargeSaveTransactionRequest.Action_Type = (int)Enumeration_Loyalty.ActionType.Save;
                objRechargeSaveTransactionRequest.Status = 0;

                objRechargeSaveTransactionRespnse = SaveMobileRechargeDetails(objRechargeSaveTransactionRequest);
                if (objRechargeSaveTransactionRespnse != null && objRechargeSaveTransactionRespnse.objRechargeSaveTransactionList != null)
                {
                    if (objRechargeSaveTransactionRespnse.ReturnValue > 0 &&
                        objRechargeSaveTransactionRespnse.objRechargeSaveTransactionList.Count > 0)
                    {
                        APIId = objRechargeSaveTransactionRespnse.objRechargeSaveTransactionList[0].ApiId;
                        APIPassword = objRechargeSaveTransactionRespnse.objRechargeSaveTransactionList[0].ApiPassword;
                        APIPin = objRechargeSaveTransactionRespnse.objRechargeSaveTransactionList[0].ApiPin;
                        rVal = objRechargeSaveTransactionRespnse.ReturnValue;

                        if (rVal > 0)
                        {
                            rechargeResult = MobileRechargeByApi(APIId, APIPin, Mobile, OperatorCode, CircleId, Amount, APIPassword, rdmNumber);
                            if (!string.IsNullOrEmpty(rechargeResult))
                            {
                                string[] arg = rechargeResult.ToString().Split('~');
                                switch (arg[0].ToLower())
                                {
                                    case SUCCESS:
                                        rechargeStatus = 1;
                                        break;
                                    case PENDING:
                                        rechargeStatus = 2;
                                        break;
                                    case FAILURE:
                                        rechargeStatus = 3;
                                        break;
                                    default:
                                        break;
                                }
                                objRechargeSaveTransactionRequest.Action_Type = (int)Enumeration_Loyalty.ActionType.Update;
                                objRechargeSaveTransactionRequest.TrxnID = rVal;
                                objRechargeSaveTransactionRequest.ApiTrxnId = Convert.ToString(arg[1]);
                                objRechargeSaveTransactionRequest.APIErrorCode = Convert.ToString(arg[2]);
                                objRechargeSaveTransactionRequest.Status = rechargeStatus;
                                objRechargeSaveTransactionRespnse = SaveMobileRechargeDetails(objRechargeSaveTransactionRequest);
                                if (objRechargeSaveTransactionRespnse != null)
                                {
                                    objRechargeSaveTransactionRespnse.ReturnValue = rechargeStatus;
                                }
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + ":: SaveMobileRechargeTransactionDetails " + ex.Message);
            }
            return objRechargeSaveTransactionRespnse;
        }

        private RechargeSaveTransactionRespnse SaveMobileRechargeDetails(RechargeSaveTransactionRequest objRechargeSaveTransactionRequest)
        {
            RechargeSaveTransactionRespnse objRechargeSaveTransactionRespnse = null;
            try
            {

                objRechargeSaveTransactionRespnse = new RechargeSaveTransactionRespnse(); //Response object
                MobileAppBO objMobileAppBO = new MobileAppBO();
                if (objRechargeSaveTransactionRequest != null)
                {
                    objRechargeSaveTransactionRequest.Mobile = objRechargeSaveTransactionRequest.Mobile;
                    objRechargeSaveTransactionRequest.LoyaltyId = objRechargeSaveTransactionRequest.LoyaltyId;
                    objRechargeSaveTransactionRequest.ActorId = objRechargeSaveTransactionRequest.ActorId;
                    objRechargeSaveTransactionRequest.OtpId = objRechargeSaveTransactionRequest.OtpId;
                    objRechargeSaveTransactionRequest.ApplicationTrnxId = objRechargeSaveTransactionRequest.ApplicationTrnxId;
                    objRechargeSaveTransactionRequest.TrxnID = objRechargeSaveTransactionRequest.TrxnID;
                    objRechargeSaveTransactionRequest.Action_Type = objRechargeSaveTransactionRequest.Action_Type;
                    objRechargeSaveTransactionRequest.Status = objRechargeSaveTransactionRequest.Status;
                    objRechargeSaveTransactionRequest.OperatorCode = objRechargeSaveTransactionRequest.OperatorCode;
                    objRechargeSaveTransactionRequest.ServiceId = objRechargeSaveTransactionRequest.ServiceId;
                    if (objRechargeSaveTransactionRequest.CircleId > 0)
                        objRechargeSaveTransactionRequest.CircleId = objRechargeSaveTransactionRequest.CircleId;
                    if (!string.IsNullOrEmpty(objRechargeSaveTransactionRequest.ApiTrxnId))
                        objRechargeSaveTransactionRequest.ApiTrxnId = objRechargeSaveTransactionRequest.ApiTrxnId;
                    if (!string.IsNullOrEmpty(objRechargeSaveTransactionRequest.APIErrorCode))
                        objRechargeSaveTransactionRequest.APIErrorCode = objRechargeSaveTransactionRequest.APIErrorCode;
                    if (!string.IsNullOrEmpty(objRechargeSaveTransactionRequest.Account))
                        objRechargeSaveTransactionRequest.Account = objRechargeSaveTransactionRequest.Account;
                    if (!string.IsNullOrEmpty(objRechargeSaveTransactionRequest.Remarks))
                        objRechargeSaveTransactionRequest.Remarks = objRechargeSaveTransactionRequest.Remarks;
                    if (objRechargeSaveTransactionRequest.Amount > 0)
                        objRechargeSaveTransactionRequest.Amount = objRechargeSaveTransactionRequest.Amount;
                    objRechargeSaveTransactionRequest.SourceMode = objRechargeSaveTransactionRequest.SourceMode;
                    objRechargeSaveTransactionRespnse = objMobileAppBO.SaveMobileRechargeTransactionDetails(objRechargeSaveTransactionRequest);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + ":: SaveMobileRechargeDetails" + ex.Message);
            }
            return objRechargeSaveTransactionRespnse;
        }

        public string MobileRechargeByApi(string userName, string userPin, string mbNumber, string oprator, int crcleCode, int amount, string paswrd, string Usertxt)
        {
            string result0;
            string result1;
            string result2;

            try
            {
                //string reqData = RECHARGE_API_URL + userName + "&pin=" + userPin + "&number="
                //    + mbNumber + "&operator=" + oprator + "&circle=" + crcleCode + "&amount=" + amount + "&usertx=" + Usertxt + "&Format=JSON";
                string reqData = RECHARGE_API_URL + userName + "&pin=" + userPin + "&number="
                     + mbNumber + "&operator=" + oprator + "&circle=" + crcleCode + "&amount=" + amount + "&usertx=" + Usertxt + "&Format=XML";

                WebClient wc = new WebClient();
                string response = wc.DownloadString(reqData);
                StringReader theReader = new StringReader(response);
                DataSet ds = new DataSet();
                ds.ReadXml(theReader);
                result0 = Convert.ToString(ds.Tables[0].Rows[0]["Status"]);
                result1 = Convert.ToString(ds.Tables[0].Rows[0]["ApiTransID"]);
                result2 = Convert.ToString(ds.Tables[0].Rows[0]["ErrorMessage"]);

                WebRequest req = WebRequest.Create(reqData);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    actualResponse = result0 + "~" + Convert.ToString(result1) + "~" + Convert.ToString(result2);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " MobileRechargeByApi() ::" + ex.Message);
            }
            return actualResponse;
        }

        public UserRetrieveResponse_Loyalty SaveAndGetOTPDetails(UserRetrieveRequest_Loyalty userRetrieveRequest)
        {
            UserRetrieveResponse_Loyalty objUserRetrieveResponse_Loyalty = null;
            try
            {
                if (userRetrieveRequest != null)
                {
                    MobileAppBO objMobileAppBO = new MobileAppBO();
                    string Random_OTP = UICommon.GetRandomNumber();
                    userRetrieveRequest.UserName = userRetrieveRequest.UserName;
                    userRetrieveRequest.Password = Random_OTP;
                    userRetrieveRequest.UserId = userRetrieveRequest.UserId;
                    userRetrieveRequest.ActionType = "Save OTP";
                    userRetrieveRequest.MobileNo = userRetrieveRequest.MobileNo;
                    userRetrieveRequest.Name = userRetrieveRequest.Name;
                    userRetrieveRequest.EmailID = userRetrieveRequest.EmailID;
                    objUserRetrieveResponse_Loyalty = objMobileAppBO.SaveAndGetOTPDetails(userRetrieveRequest);
                    if (objUserRetrieveResponse_Loyalty != null)
                    {
                        if (objUserRetrieveResponse_Loyalty.ReturnValue > 0)
                        {
                            if (!string.IsNullOrEmpty(Random_OTP))
                            {
                                objUserRetrieveResponse_Loyalty.ReturnValue = objUserRetrieveResponse_Loyalty.ReturnValue;
                                objUserRetrieveResponse_Loyalty.ReturnMessage = Convert.ToString(Random_OTP);
                                userRetrieveRequest.Password = Random_OTP;

                                if (userRetrieveRequest.OTPAlertType == "Email")
                                    SendOtpInEmail(userRetrieveRequest);
                                else
                                    SendOTPToCustomer(userRetrieveRequest);

                                if (userRetrieveRequest.OrderDispatchID > 0)
                                {
                                    if (DOMAIN == UIConstants.MILLER_CLUB)
                                        SendOTPToMerchant(userRetrieveRequest);
                                }
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + ":: SaveAndGetOTPDetails()" + ex.Message);
            }
            return objUserRetrieveResponse_Loyalty;
        }

        private void SendOTPToCustomer(UserRetrieveRequest_Loyalty userRetrieveRequest)
        {

            try
            {
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                if (userRetrieveRequest.OTPType == "Enrollment")
                    ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.OTPForEnrollmentAuthentication);
                else
                    ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.OTPForRewardCardsENCashAuthorization);
                ObjSmsEmailRetrieveRequest.Type = UIConstants.SMS;
                ObjSmsEmailRetrieveRequest.UserName = MERCHANT_USERNAME;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = userRetrieveRequest.MobileNo;
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponseSMS = mobileAppBO.GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);
                for (int i = 0; i < ObjSmsEmailRetrieveResponseSMS.lstSmsEmailDetails.Count; i++)
                {
                    List<AlertTemplateDynamicContent> DynamicTemplate = new List<AlertTemplateDynamicContent>(){
                new AlertTemplateDynamicContent (CUSTOMER_NAME,userRetrieveRequest.MerchantUserName),
                new AlertTemplateDynamicContent (AUTHRIZEDNO, userRetrieveRequest.Password),
                new AlertTemplateDynamicContent (MEMBER_NAME , userRetrieveRequest.Name)};

                    AlertUtiltityParameters alertUtiltityParameters = new AlertUtiltityParameters
                   (ObjSmsEmailRetrieveResponseSMS.lstSmsEmailDetails[i].Type, userRetrieveRequest.MobileNo, DynamicTemplate, ObjSmsEmailRetrieveResponseSMS.lstSmsEmailDetails[i]);

                    if (DOMAIN == UIConstants.CCRLP)
                    {
                        alertUtiltityParameters.VendorMethodName = SMSVendorMethodEnum.Unifornic;
                        alertUtiltityParameters.IsGetRequest = true;
                    }

                    else if (DOMAIN == UIConstants.GodrejLocks)
                        alertUtiltityParameters.IsGetRequest = true;
                    else if (DOMAIN == UIConstants.MILLER_CLUB)
                        alertUtiltityParameters.VendorMethodName = SMSVendorMethodEnum.Skylinesms;
                    else if (DOMAIN == UIConstants.WALK_KARO)
                    {
                        alertUtiltityParameters.VendorMethodName = SMSVendorMethodEnum.SmsMSG91;
                        alertUtiltityParameters.objSmsEmail.Var1 = userRetrieveRequest.Password;
                        alertUtiltityParameters.objSmsEmail.Mobile = "91" + userRetrieveRequest.MobileNo;
                    }
                    try
                    {
                        var Result = SendAlertUtility.SendAlert(alertUtiltityParameters);
                    }
                    catch (Exception ex1)
                    {
                        ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + ":: SendOTPToCustomer()" + ex1.Message);
                    }
                }

                //For sending OTP in AIC project at the time of redemption.
                if (DOMAIN == UIConstants.AIC)
                {
                    if (ObjSmsEmailRetrieveRequest.TemplateName == Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.OTPForRewardCardsENCashAuthorization))
                    {

                        ObjSmsEmailRetrieveRequest.Type = UIConstants.EMAIL;
                        SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponseEmail = mobileAppBO.GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);
                        for (int i = 0; i < ObjSmsEmailRetrieveResponseEmail.lstSmsEmailDetails.Count; i++)
                        {
                            List<AlertTemplateDynamicContent> DynamicTemplateEmail = new List<AlertTemplateDynamicContent>(){
                        new AlertTemplateDynamicContent (CUSTOMER_NAME,userRetrieveRequest.Name),
                        new AlertTemplateDynamicContent (AUTHRIZEDNO, userRetrieveRequest.Password),
                        new AlertTemplateDynamicContent (MEMBER_NAME , userRetrieveRequest.Name)};

                            AlertUtiltityParameters alertUtiltityParametersEmail = new AlertUtiltityParameters
                           (UIConstants.EMAIL, userRetrieveRequest.EmailID, DynamicTemplateEmail, ObjSmsEmailRetrieveResponseEmail.lstSmsEmailDetails[i]);

                            alertUtiltityParametersEmail.ShouldUseThreading = false;
                            alertUtiltityParametersEmail.MailHeaderText = UIConstants.OTP_ENROLLMENT_HEADER;
                            SendAlertUtility.SendAlert(alertUtiltityParametersEmail);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + ":: SendOTPToCustomer()" + ex.Message);
            }
        }

        private void SendOTPToMerchant(UserRetrieveRequest_Loyalty userRetrieveRequest)
        {
            try
            {
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.OtpRequestAlertToMerchant);
                ObjSmsEmailRetrieveRequest.Type = UIConstants.SMS;
                ObjSmsEmailRetrieveRequest.UserName = MERCHANT_USERNAME;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = MERCHANT_MOBILE_NO;
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponseSMS = mobileAppBO.GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);
                for (int i = 0; i < ObjSmsEmailRetrieveResponseSMS.lstSmsEmailDetails.Count; i++)
                {
                    List<AlertTemplateDynamicContent> DynamicTemplate = new List<AlertTemplateDynamicContent>(){
                new AlertTemplateDynamicContent ("[User Name]",MERCHANT_USERNAME),
                new AlertTemplateDynamicContent ("[PIN]", userRetrieveRequest.Password)
                    };

                    AlertUtiltityParameters alertUtiltityParameters = new AlertUtiltityParameters
                   (ObjSmsEmailRetrieveResponseSMS.lstSmsEmailDetails[i].Type, MERCHANT_MOBILE_NO, DynamicTemplate, ObjSmsEmailRetrieveResponseSMS.lstSmsEmailDetails[i]);


                    alertUtiltityParameters.VendorMethodName = SMSVendorMethodEnum.Skylinesms;
                    try
                    {
                        var Result = SendAlertUtility.SendAlert(alertUtiltityParameters);
                    }
                    catch (Exception ex1)
                    {
                        ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + ":: SendOTPToMerchant()" + ex1.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + ":: SendOTPToMerchant()" + ex.Message);
            }
        }

        #endregion

        #region Recharge and Utility Payments

        public RechargeSaveTransactionRespnse SaveRechargeUtilityPaymentsDetails(RechargeSaveTransactionRequest objRechargeSaveTransactionRequest)
        {
            RechargeSaveTransactionRespnse objRechargeSaveTransactionRespnse = null;
            string MOBILE_RECHARGE = "Mobile Recharge", UTILITY_PAYMENTS = "Utility Payments";
            string RechargeResult = string.Empty;
            try
            {
                //Description :
                //RECHARGE_STATUS= 0 ----------Default
                //RECHARGE_STATUS= 1 ----------Success
                //RECHARGE_STATUS= 2 ----------Pending
                //RECHARGE_STATUS= 3 ----------Failure

                int rechargeStatus = -1, rVal = 0;
                string rechargeResult = string.Empty, Account = string.Empty, Recharge_type = string.Empty;

                if (objRechargeSaveTransactionRequest != null)
                {
                    Mobile = objRechargeSaveTransactionRequest.Mobile;
                    OperatorCode = objRechargeSaveTransactionRequest.OperatorCode;
                    CircleId = objRechargeSaveTransactionRequest.CircleId;
                    Amount = objRechargeSaveTransactionRequest.Amount;
                    Account = objRechargeSaveTransactionRequest.Account;
                    if (objRechargeSaveTransactionRequest.ServiceId == 1 || objRechargeSaveTransactionRequest.ServiceId == 4)
                        Recharge_type = MOBILE_RECHARGE;
                    else
                        Recharge_type = UTILITY_PAYMENTS;
                }
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                var random = new Random();
                string rdmNumber = new string(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray());

                objRechargeSaveTransactionRequest.ApplicationTrnxId = rdmNumber;
                objRechargeSaveTransactionRequest.TrxnID = rVal;
                objRechargeSaveTransactionRequest.Action_Type = (int)Enumeration_Loyalty.ActionType.Save;
                objRechargeSaveTransactionRequest.Status = 0;
                objRechargeSaveTransactionRespnse = mobileAppBO.SaveMobileAndUtilityRechargeTranDetails(objRechargeSaveTransactionRequest);
                rVal = objRechargeSaveTransactionRespnse.ReturnValue;

                if (rVal > 0)
                {
                    rechargeResult = RechargeAndUtilityPaymentsAPI(Mobile, OperatorCode, Amount, Recharge_type, Account);
                    if (!string.IsNullOrEmpty(rechargeResult))
                    {
                        string[] arg = rechargeResult.ToString().Split('~');
                        if (rechargeResult.Split('~')[0] == UIConstants.VALUE_ZERO)
                        {
                            if (rechargeResult.Split('~')[1] == "Your recharge request is accepted.Success" || rechargeResult.Split('~')[1] == "Your recharge request is accepted.Pending")
                                rechargeStatus = 2;

                            else if (rechargeResult.Split('~')[1] == "Your recharge request is accepted.Failure")
                                rechargeStatus = 3;
                        }
                        else if (rechargeResult.Split('~')[0] == UIConstants.VALUE_ONE)
                        {
                            rechargeStatus = 3;
                        }
                        else
                        {
                            rechargeStatus = 4;
                        }
                        objRechargeSaveTransactionRequest.Action_Type = (int)Enumeration_Loyalty.ActionType.Update;
                        objRechargeSaveTransactionRequest.TrxnID = rVal;
                        objRechargeSaveTransactionRequest.ApiTrxnId = rechargeResult.Split('~')[2];
                        objRechargeSaveTransactionRequest.APIErrorCode = rechargeResult.Split('~')[0];
                        objRechargeSaveTransactionRequest.Status = rechargeStatus;
                        objRechargeSaveTransactionRequest.Remarks = rechargeResult.Split('~')[1];
                        objRechargeSaveTransactionRespnse = mobileAppBO.SaveMobileAndUtilityRechargeTranDetails(objRechargeSaveTransactionRequest);
                        if (objRechargeSaveTransactionRespnse != null)
                        {
                            objRechargeSaveTransactionRespnse.ReturnValue = rechargeStatus;
                            objRechargeSaveTransactionRespnse.ReturnMessage = rechargeResult.Split('~')[1];
                        }
                    }
                }
                else
                    objRechargeSaveTransactionRespnse.ReturnValue = rechargeStatus;
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + ":: SaveRechargeUtilityPaymentsDetails " + ex.Message);
            }
            return objRechargeSaveTransactionRespnse;
        }

        //After successful Validation Recharge Request sending to API.
        private string RechargeAndUtilityPaymentsAPI(string Number, string oprator, int amount, string Recharge_type, string Account = null)
        {
            string TokenKey = string.Empty, JsonResponse = string.Empty, Message = string.Empty, RECHARGE_ID = string.Empty, RECHARGE_STATUS = string.Empty,
                   ReturnMessage = string.Empty, APIRequiredURL = string.Empty, BILL_VERIFY_ID = string.Empty, ERROR_CODE = "3", TimeStamp = string.Empty,
                   MOBILE_RECHARGE = "Mobile Recharge", UTILITY_PAYMENTS = "Utility Payments";
            HttpWebResponse httpWebResponce = null; JObject jObject = null;
            try
            {
                if (Recharge_type == MOBILE_RECHARGE)
                {
                    TimeStamp = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                    if (TimeStamp != string.Empty)
                    {
                        // Url to get TOKEN from API
                        APIRequiredURL = TOKEN_KEY_URL + "TimeStamp:" + TimeStamp + "&secret_key=" + SECREATE_KEY + "&email_id=" + API_MAIL_ID;
                        httpWebResponce = RechargeGETMethod(APIRequiredURL);
                        // API Responce
                        jObject = RechargeJsonResponse(httpWebResponce);
                        // Getting TOKEN value from API responce
                        TokenKey = Convert.ToString(jObject["encode_token"]);

                        // Url to Recharge/Utility Payments from API
                        APIRequiredURL = RECHARGE_API_URL + "username=" + API_USER_NAME + "&pwd=" + API_PASSWORD + "&operatorcode=" + oprator + "&number=" + Number + "&amount=" + amount + "&client_id=" + CLIENT_ID + "&token=" + TokenKey;
                        httpWebResponce = RechargeGETMethod(APIRequiredURL);
                        // Getting Recharge details from API responce
                        jObject = RechargeJsonResponse(httpWebResponce);
                        Message = Convert.ToString(jObject["message"]);
                        RECHARGE_ID = Convert.ToString(jObject["recharge_id"]);
                        RECHARGE_STATUS = Convert.ToString(jObject["recharge_status"]);
                        ERROR_CODE = Convert.ToString(jObject["error_code"]);
                        ReturnMessage = ERROR_CODE + "~" + Message + RECHARGE_STATUS + "~" + RECHARGE_ID;
                    }
                    else
                    {
                        ReturnMessage = ERROR_CODE + "~" + Message + RECHARGE_STATUS;
                    }
                }
                else if (Recharge_type == UTILITY_PAYMENTS)
                {
                    TimeStamp = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                    if (TimeStamp != string.Empty)
                    {
                        // Url to get TOKEN from API
                        APIRequiredURL = TOKEN_KEY_URL + "TimeStamp:" + TimeStamp + "&secret_key=" + SECREATE_KEY + "&email_id=" + API_MAIL_ID;
                        httpWebResponce = RechargeGETMethod(APIRequiredURL);
                        // API Responce
                        jObject = RechargeJsonResponse(httpWebResponce);
                        // Getting TOKEN value from API responce
                        TokenKey = Convert.ToString(jObject["encode_token"]);

                        httpWebResponce = null;
                        jObject = null;
                        if (Account != string.Empty) // Url to get BILL_VERIFY_ID from API using Account Number
                            APIRequiredURL = CHECK_BILLE_API_URL + "username=" + API_USER_NAME + "&pwd=" + API_PASSWORD + "&operatorcode=" + oprator + "&number=" + Number + "&account=" + Account + "&amount=" + amount + "&token=" + TokenKey;

                        else  // Url to get BILL_VERIFY_ID from API 
                            APIRequiredURL = CHECK_BILLE_API_URL + "username=" + API_USER_NAME + "&pwd=" + API_PASSWORD + "&operatorcode=" + oprator + "&number=" + Number + "&amount=" + amount + "&token=" + TokenKey;

                        httpWebResponce = RechargeGETMethod(APIRequiredURL);
                        // API Responce
                        jObject = RechargeJsonResponse(httpWebResponce);
                        // Getting Bill_Number value from API responce
                        BILL_VERIFY_ID = Convert.ToString(jObject["verify_ref_id"]);
                    }
                    if (BILL_VERIFY_ID != string.Empty)
                    {
                        httpWebResponce = null; jObject = null; TokenKey = string.Empty;
                        TimeStamp = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                        // Url to get TOKEN from API
                        APIRequiredURL = TOKEN_KEY_URL + "TimeStamp:" + TimeStamp + "&secret_key=" + SECREATE_KEY + "&email_id=" + API_MAIL_ID;
                        httpWebResponce = RechargeGETMethod(APIRequiredURL);
                        // API Responce
                        jObject = RechargeJsonResponse(httpWebResponce);
                        // Getting TOKEN value from API responce
                        TokenKey = Convert.ToString(jObject["encode_token"]);

                        if (Account != string.Empty) // Url to get BILL_VERIFY_ID from API using Account Number
                        {
                            APIRequiredURL = RECHARGE_API_URL + "username=" + API_USER_NAME + "&pwd=" + API_PASSWORD + "&operatorcode=" + oprator + "&number=" + Number + "&account=" + Account + "&amount=" + amount + "&bill_verify_id=" + BILL_VERIFY_ID + "&client_id=" + CLIENT_ID + "&token=" + TokenKey;
                        }
                        else   // Url to Recharge/Utility Payments from API 
                        {
                            APIRequiredURL = RECHARGE_API_URL + "username=" + API_USER_NAME + "&pwd=" + API_PASSWORD + "&operatorcode=" + oprator + "&number=" + Number + "&amount=" + amount + "&bill_verify_id=" + BILL_VERIFY_ID + "&client_id=" + CLIENT_ID + "&token=" + TokenKey;
                        }
                        httpWebResponce = RechargeGETMethod(APIRequiredURL);
                        jObject = RechargeJsonResponse(httpWebResponce);
                        // Getting Recharge details from API responce
                        Message = Convert.ToString(jObject["message"]);
                        RECHARGE_ID = Convert.ToString(jObject["recharge_id"]);
                        RECHARGE_STATUS = Convert.ToString(jObject["recharge_status"]);
                        ERROR_CODE = Convert.ToString(jObject["error_code"]);
                        ReturnMessage = ERROR_CODE + "~" + Message + RECHARGE_STATUS + "~" + RECHARGE_ID;
                    }
                    else
                        ReturnMessage = ERROR_CODE + "~" + Message + RECHARGE_STATUS;
                }
            }
            catch (Exception ex)
            {
                ReturnMessage = ERROR_CODE + "~" + Message + RECHARGE_STATUS;
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "RechargeAPImethod " + ex.Message);
            }
            return ReturnMessage;
        }

        private JObject RechargeJsonResponse(HttpWebResponse Final)
        {
            string JsonResponse = string.Empty; JObject RechargeResponse = null;
            try
            {
                using (Stream responseStream = Final.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    JsonResponse = reader.ReadToEnd().ToString();
                }
                RechargeResponse = JObject.Parse(JsonResponse);
            }

            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "RechargeJsonResponse() :: " + ex.Message);
            }
            return RechargeResponse;
        }

        private HttpWebResponse RechargeGETMethod(String Url)
        {
            HttpWebResponse response = null;
            try
            {
                HttpWebRequest tRequest = WebRequest.Create(Url) as HttpWebRequest;
                tRequest.ContentType = "application/json";
                tRequest.Accept = "*/*";
                response = tRequest.GetResponse() as HttpWebResponse;
            }
            catch (WebException ex)
            {
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    ErrorHandler.WriteErrorToPhysicalPath(reader.ReadToEnd());
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "RechargeGETMethod() :: " + ex.Message);
            }
            return response;
        }

        #endregion

        #region Refere A Purchase
        //For Sending the Sms to Newly reffered customer
        public SmsEmailRetrieveResponse SendSMSToNewCustomer(SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest)
        {

            SmsEmailRetrieveResponse objSmsEmailRetrieveResponse = null;
            int ReturnValue = 0;
            try
            {

                ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.Referral_to_new_customer);
                ObjSmsEmailRetrieveRequest.Type = UIConstants.SMS;
                ObjSmsEmailRetrieveRequest.UserName = ObjSmsEmailRetrieveRequest.UserName;
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse = mobileAppBO.SendSMSToNewCustomer(ObjSmsEmailRetrieveRequest);
                if (ObjSmsEmailRetrieveResponse != null && ObjSmsEmailRetrieveResponse.lstSmsEmailDetails != null && ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count > 0)
                {
                    StringBuilder templat = new StringBuilder();
                    templat.Append(ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[0].Template);
                    templat.Replace("[Referrer]", ObjSmsEmailRetrieveRequest.ReferingCustomerName); //Referer: Person who is referring new customer
                    templat.Replace("[recommended product]", ObjSmsEmailRetrieveRequest.ProductInterested);
                    StringBuilder data = new StringBuilder();

                    data.Append(UIConstants.WORKING_KEY + workingkey);
                    data.Append(UIConstants.SMS_TO + ObjSmsEmailRetrieveRequest.ReferredCustomerMob); //New customer Mobile No.
                    data.Append(UIConstants.SMS_SENDER + smssender);
                    data.Append("&message=" + templat);


                    string directory = System.IO.Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
                    directory = directory.Replace(@"bin\Debug", @"ErrorFolders\ErrorLog\");
                    string path = directory + DateTime.Today.ToString("dd-MM-yy") + ".txt";
                    UICommon.Send_Sms(Convert.ToString(data), path);
                    ReturnValue = 1;
                }
                else
                    ReturnValue = -1;

            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + ":: SendSMSToNewCustomer()" + ex.Message);
            }
            objSmsEmailRetrieveResponse = new SmsEmailRetrieveResponse();
            objSmsEmailRetrieveResponse.ReturnValue = ReturnValue;
            return objSmsEmailRetrieveResponse;
        }

        //For Sending the Sms to Referring customer.
        public SmsEmailRetrieveResponse SendSMStoReferrer(SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest)
        {
            int rVal = 0;
            SmsEmailRetrieveResponse objSmsEmailRetrieveResponse = null;
            try
            {

                ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.Referral_to_Referrer);
                ObjSmsEmailRetrieveRequest.Type = UIConstants.SMS;
                ObjSmsEmailRetrieveRequest.UserName = ObjSmsEmailRetrieveRequest.UserName;
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse = mobileAppBO.SendSMStoReferrer(ObjSmsEmailRetrieveRequest);
                if (ObjSmsEmailRetrieveResponse != null && ObjSmsEmailRetrieveResponse.lstSmsEmailDetails != null && ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count > 0)
                {
                    StringBuilder templat = new StringBuilder();
                    templat.Append(ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[0].Template);
                    StringBuilder data = new StringBuilder();

                    string Mobile = ObjSmsEmailRetrieveRequest.RefferingCustMob;
                    data.Append(UIConstants.WORKING_KEY + workingkey);
                    data.Append(UIConstants.SMS_TO + Mobile);
                    data.Append(UIConstants.SMS_SENDER + smssender);
                    data.Append("&message=" + templat);


                    string directory = System.IO.Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
                    directory = directory.Replace(@"bin\Debug", @"ErrorFolders\ErrorLog\");
                    string path = directory + DateTime.Today.ToString("dd-MM-yy") + ".txt";
                    UICommon.Send_Sms(Convert.ToString(data), path);
                    rVal = 1;
                }
                else
                    rVal = -1;
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + ":: SendSMStoReferrer()" + ex.Message);
            }
            objSmsEmailRetrieveResponse = new SmsEmailRetrieveResponse();
            objSmsEmailRetrieveResponse.ReturnValue = rVal;
            return objSmsEmailRetrieveResponse;
        }

        //For Sending the Sms to Teritory Manager.
        public SmsEmailRetrieveResponse SendSmsToTeritoryManager(SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest)
        {
            int rVal = 0;
            SmsEmailRetrieveResponse objSmsEmailRetrieveResponse = null;
            try
            {

                ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.Sms_To_Teritory_Manager);
                ObjSmsEmailRetrieveRequest.Type = UIConstants.SMS;
                ObjSmsEmailRetrieveRequest.UserName = ObjSmsEmailRetrieveRequest.UserName;
                ObjSmsEmailRetrieveRequest.ReferenceId = ObjSmsEmailRetrieveRequest.ReferenceId;
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse = mobileAppBO.SendSmsToTeritoryManager(ObjSmsEmailRetrieveRequest);
                if (ObjSmsEmailRetrieveResponse != null)
                {
                    if (ObjSmsEmailRetrieveResponse.lstSmsEmailDetails != null && ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count > 1)
                    {
                        StringBuilder templat = new StringBuilder();
                        templat.Append(ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[0].Template);
                        templat.Replace("[Referrence_Cust_name]", ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[1].RefCustName);
                        templat.Replace("[Reference_Cust_Mobile]", ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[1].RefCustMob);
                        templat.Replace("[Dealername]", ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[1].DealerName);
                        templat.Replace("[UserName]", ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[1].DealerUserName);
                        templat.Replace("[Reference_Created date]", Convert.ToString(UICommon.MMDDtoDDMM(ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[1].RefCreatedDate.ToString())));
                        StringBuilder data = new StringBuilder();

                        if (ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[1].TeritoryMgrMob != null)
                        {
                            string Mobile = ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[1].TeritoryMgrMob;
                            data.Append(UIConstants.WORKING_KEY + workingkey);
                            data.Append(UIConstants.SMS_TO + Mobile);
                            data.Append(UIConstants.SMS_SENDER + smssender);
                            data.Append("&message=" + templat);

                            string directory = System.IO.Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
                            directory = directory.Replace(@"bin\Debug", @"ErrorFolders\ErrorLog\");
                            string path = directory + DateTime.Today.ToString("dd-MM-yy") + ".txt";
                            UICommon.Send_Sms(Convert.ToString(data), path);
                            rVal = 1;
                        }
                        else
                            rVal = -1;
                    }
                    else
                        rVal = -1;
                }
                else
                    rVal = -1;
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + ":: SendSmsToTeritoryManager()" + ex.Message);
            }
            objSmsEmailRetrieveResponse = new SmsEmailRetrieveResponse();
            objSmsEmailRetrieveResponse.ReturnValue = rVal;
            return objSmsEmailRetrieveResponse;
        }

        #endregion

        public bool SendSMSForSuccessfulRedemptionMobileApp(UserRedemption_Details objUserRedemption_Details)
        {
            bool Result = false;
            try
            {
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();


                ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.PointDebitFromLocation_EXE);
                ObjSmsEmailRetrieveRequest.Type = UIConstants.SMS;
                ObjSmsEmailRetrieveRequest.UserName = objUserRedemption_Details.LoyaltyID;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = objUserRedemption_Details.Mobile;
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponseSMS = mobileAppBO.GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);

                ObjSmsEmailRetrieveRequest.Type = UIConstants.EMAIL;
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponseEMAIL = mobileAppBO.GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);


                List<AlertTemplateDynamicContent> DynamicTemplate = new List<AlertTemplateDynamicContent>(){
                    new AlertTemplateDynamicContent ("[client name]",objUserRedemption_Details.CustomerName),
                    new AlertTemplateDynamicContent ("[points]",Convert.ToString(objUserRedemption_Details.RedeemedPoint)),
                    new AlertTemplateDynamicContent ("[BALANCE]",Convert.ToString(objUserRedemption_Details.PointBalance-objUserRedemption_Details.RedeemedPoint)),
                    new AlertTemplateDynamicContent ("[PRODUCT_NAME]",Convert.ToString(objUserRedemption_Details.ProductName))
                };

                for (int i = 0; i < ObjSmsEmailRetrieveResponseSMS.lstSmsEmailDetails.Count; i++)
                {

                    AlertUtiltityParameters alertUtiltityParametersSMS = new AlertUtiltityParameters
                   (ObjSmsEmailRetrieveResponseSMS.lstSmsEmailDetails[i].Type, objUserRedemption_Details.Mobile, DynamicTemplate, ObjSmsEmailRetrieveResponseSMS.lstSmsEmailDetails[i]);
                    alertUtiltityParametersSMS.IsGetRequest = true;

                    if (DOMAIN == UIConstants.CCRLP)
                    {
                        alertUtiltityParametersSMS.VendorMethodName = SMSVendorMethodEnum.Unifornic;
                        alertUtiltityParametersSMS.IsGetRequest = true;
                    }

                    try
                    {
                        var AlertResult = SendAlertUtility.SendAlert(alertUtiltityParametersSMS);
                        Result = true;
                    }
                    catch (Exception ex1)
                    {
                        Result = false;
                        ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + ":: SendSMSForSuccessfulRedemptionMobileApp()" + ex1.Message);
                    }
                }

                if (DOMAIN == UIConstants.AIC)
                {

                    for (int i = 0; i < ObjSmsEmailRetrieveResponseEMAIL.lstSmsEmailDetails.Count; i++)
                    {

                        AlertUtiltityParameters alertUtiltityParametersEMAIL = new AlertUtiltityParameters
                        (UIConstants.EMAIL, objUserRedemption_Details.EmailID, DynamicTemplate, ObjSmsEmailRetrieveResponseEMAIL.lstSmsEmailDetails[i]);
                        alertUtiltityParametersEMAIL.MailHeaderText = UIConstants.REDEMPTION_HEADER;
                        var AlertResult = SendAlertUtility.SendAlert(alertUtiltityParametersEMAIL);
                    }
                }
            }
            catch (Exception ex)
            {
                Result = false;
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + ":: SendSMSForSuccessfulRedemptionMobileApp()" + ex.Message);
            }

            return Result;
        }

        //To issue giftcard Automatically for 1st login of a Customer through mobile app
        public void IssueAutomaticGiftcard(string UserName, string GiftCardActionType, int GiftCardActionID)
        {
            try
            {
                string Password = string.Empty;
                int AutoCardIssueID = Convert.ToInt32(UIConstants.VALUE_ZERO);
                string FullName = string.Empty;
                string Email = string.Empty;
                string SelectedGiftCardName = string.Empty;
                string CardNumber = string.Empty;
                string CardPinType = string.Empty;
                string Mobile = string.Empty;
                string Prefix = string.Empty;
                string MerchantUserName = string.Empty;

                decimal CardBalance = 0;
                string Exp_Date = string.Empty;

                Password = UICommon.GetRandomNumber();

                IssueGiftCardAutomaticallyRetrieveRequest objIssueGiftCardAutomaticallyRetrieveRequest = new IssueGiftCardAutomaticallyRetrieveRequest();

                objIssueGiftCardAutomaticallyRetrieveRequest.UserName = UserName;
                objIssueGiftCardAutomaticallyRetrieveRequest.GiftCardPin = UICommon.Encrypt(Password);
                objIssueGiftCardAutomaticallyRetrieveRequest.GiftCardActionType = GiftCardActionType;
                objIssueGiftCardAutomaticallyRetrieveRequest.GiftCardActionID = GiftCardActionID;

                IssueGiftCardAutomaticallyRetrieveResponse objIssueGiftCardAutomaticallyRetrieveResponse = mobileAppBO.IssueAutomaticGiftcardForFirstLogin(objIssueGiftCardAutomaticallyRetrieveRequest);

                string[] arg = null;

                if (objIssueGiftCardAutomaticallyRetrieveResponse != null)
                {
                    if (objIssueGiftCardAutomaticallyRetrieveResponse.ReturnMessage.Contains(":"))
                    {
                        arg = objIssueGiftCardAutomaticallyRetrieveResponse.ReturnMessage.ToString().Split(':');
                        AutoCardIssueID = Convert.ToInt32(arg[0]);
                        FullName = arg[1];
                        Email = arg[2];
                        SelectedGiftCardName = arg[3];
                        CardNumber = arg[4];
                        CardPinType = arg[5];
                        Mobile = arg[6];
                        Prefix = arg[7];
                        MerchantUserName = arg[8];
                    }
                    if ((objIssueGiftCardAutomaticallyRetrieveResponse != null) && (AutoCardIssueID > 0))
                    {

                        GiftCardIssueRetrieveRequest ObjGiftCardIssueRetrieveRequest = new GiftCardIssueRetrieveRequest();
                        ObjGiftCardIssueRetrieveRequest.CardIssueId = AutoCardIssueID;
                        GiftCardIssueRetrieveResponse ObjGiftCardIssueRetrieveResponse = mobileAppBO.GetCardBalanceAndExpiryDate(ObjGiftCardIssueRetrieveRequest);
                        if (ObjGiftCardIssueRetrieveResponse.LstIssuedGiftCardList.Count > 0)
                        {
                            CardBalance = ObjGiftCardIssueRetrieveResponse.LstIssuedGiftCardList[0].Balance;
                            Exp_Date = ObjGiftCardIssueRetrieveResponse.LstIssuedGiftCardList[0].ExpiryStatus;
                        }

                        if (DOMAIN != UIConstants.GREEN_COVER)
                            GE_SMSMail_RecepientMobileApp(Email, Mobile, FullName, CardPinType, CardNumber, SelectedGiftCardName, MerchantUserName, UserName, AutoCardIssueID, Prefix);
                    }

                }
            }
            catch (Exception ex)
            {

                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " IssueAutomaticGiftcard() " + ex.Message);
            }

        }
        // end

        public List<string> GetSmtp(string MerchantName)
        {
            List<string> lstsmtp = new List<string>();
            try
            {
                MobileAppBO objMobileAppBO = new MobileAppBO();
                SMTP_SMS_DetailsRetriveRequest objSMTP_SMS_DetailsRetriveRequest = new SMTP_SMS_DetailsRetriveRequest();
                objSMTP_SMS_DetailsRetriveRequest.MerchantName = MerchantName;
                SMTP_SMS_DetailsRetriveResponse objSMTP_SMS_DetailsRetriveResponse = objMobileAppBO.GetSMTPDetailsForSendMailByMerchantId(objSMTP_SMS_DetailsRetriveRequest);// SMTP.GetSmtp(MerchantName);
                {
                    if (objSMTP_SMS_DetailsRetriveResponse.lstSMSServiceprovider.Count > 0)
                    {
                        objSMTP_SMS_DetailsRetriveResponse.lstSMSServiceprovider = objSMTP_SMS_DetailsRetriveResponse.lstSMSServiceprovider;
                        lstsmtp.Add(Convert.ToString(objSMTP_SMS_DetailsRetriveResponse.lstSMSServiceprovider[0].SMTPAddress));
                        lstsmtp.Add(Convert.ToString(objSMTP_SMS_DetailsRetriveResponse.lstSMSServiceprovider[0].SMTPUserName));
                        lstsmtp.Add(Convert.ToString(objSMTP_SMS_DetailsRetriveResponse.lstSMSServiceprovider[0].SMTPPassword));
                        lstsmtp.Add(Convert.ToString(objSMTP_SMS_DetailsRetriveResponse.lstSMSServiceprovider[0].FromEmail));
                        lstsmtp.Add(Convert.ToString(objSMTP_SMS_DetailsRetriveResponse.lstSMSServiceprovider[0].SMSSender));
                        lstsmtp.Add(Convert.ToString(objSMTP_SMS_DetailsRetriveResponse.lstSMSServiceprovider[0].SMSWorkingKey));
                        lstsmtp.Add(Convert.ToString(objSMTP_SMS_DetailsRetriveResponse.lstSMSServiceprovider[0].SmsURL));
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetSmtp() " + ex.Message);
            }
            return lstsmtp;

        }

        public SaveAndGetProjectsRetrieveResponse GetProjectDetailsByKey(SaveAndGetProjectsRetrieveRequest objSaveAndGetProjectsRetrieveRequest)
        {
            SaveAndGetProjectsRetrieveResponse objSaveAndGetProjectsRetrieveResponse = null;

            try
            {
                MobileAppBO objMobileAppBO = new MobileAppBO();
                objSaveAndGetProjectsRetrieveResponse = objMobileAppBO.GetProjectDetailsByKey(objSaveAndGetProjectsRetrieveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetProjectDetailsByKey() " + ex.Message);
            }
            return objSaveAndGetProjectsRetrieveResponse;
        }

        #region Catalogue Redemption Alert via SMS & EMAIL to Admin

        public CatalogueRedemptionAlertResponseJson SendCatalogueRedemptionAlertMobileApp(CatalogueRedemptionAlertRequestJson objCatalogueRedemptionAlertRequest)
        {
            CatalogueRedemptionAlertResponseJson objCatalogueRedemptionAlertResponse = new CatalogueRedemptionAlertResponseJson();
            try
            {

                if (objCatalogueRedemptionAlertRequest != null)
                {
                    if (objCatalogueRedemptionAlertRequest.MerchantID != 0
                        && objCatalogueRedemptionAlertRequest.ObjCatalogueList != null
                        && objCatalogueRedemptionAlertRequest.ObjCustShippingAddressDetails != null
                        && (!string.IsNullOrEmpty(objCatalogueRedemptionAlertRequest.MerchantMobileNo))
                        && (!string.IsNullOrEmpty(objCatalogueRedemptionAlertRequest.MerchantEmailID))
                        && (!string.IsNullOrEmpty(objCatalogueRedemptionAlertRequest.UserName)))
                    {
                        MobileAppBO objMobileAppBO = new MobileAppBO();
                        MerchantConfigurationRetriveRequest objMerchantConfigurationRetriveRequest = new MerchantConfigurationRetriveRequest();
                        objMerchantConfigurationRetriveRequest.MerchantID = objCatalogueRedemptionAlertRequest.MerchantID;
                        MerchantConfigurationRetriveResponse objMerchantConfigurationRetriveResponse = objMobileAppBO.BindMerchatConfigureDetailsMobileApp(objMerchantConfigurationRetriveRequest);
                        if (objMerchantConfigurationRetriveResponse != null && objMerchantConfigurationRetriveResponse.lstMerchantConfigRetriveList != null && objMerchantConfigurationRetriveResponse.lstMerchantConfigRetriveList.Count > 0)
                        {
                            var CustomerList = objMerchantConfigurationRetriveResponse.lstMerchantConfigRetriveList.Where(a => a.ModuleName == "CATALOGUE_REDEMPTION_ALERT").ToList();
                            for (int i = 0; i < CustomerList.Count; i++)
                            {
                                if (CustomerList[i].ControlName.ToString() == "EMAIL")
                                {
                                    if (CustomerList[i].Is_Visible)
                                    {
                                        string[] arg = SendSMSAndEmailCatalogueRedemptionAlert(objCatalogueRedemptionAlertRequest, false, true).ToString().Split('~'); //For Email
                                        objCatalogueRedemptionAlertResponse.ReturnMessage = arg[1];
                                        objCatalogueRedemptionAlertResponse.ReturnValue = Convert.ToInt32(arg[0]);
                                    }
                                    else
                                    {
                                        objCatalogueRedemptionAlertResponse.ReturnMessage = "Email will not send because it is not configured from merchant control page.";
                                        objCatalogueRedemptionAlertResponse.ReturnValue = 0;
                                    }
                                }
                                else if (CustomerList[i].ControlName.ToString() == "SMS")
                                {
                                    if (CustomerList[i].Is_Visible)
                                    {
                                        string[] arg = SendSMSAndEmailCatalogueRedemptionAlert(objCatalogueRedemptionAlertRequest, true, false).ToString().Split('~'); //For SMS
                                        objCatalogueRedemptionAlertResponse.ReturnMessage = arg[1];
                                        objCatalogueRedemptionAlertResponse.ReturnValue = Convert.ToInt32(arg[0]);
                                    }
                                    else
                                    {
                                        objCatalogueRedemptionAlertResponse.ReturnMessage = "SMS will not send because it is not configured from merchant control page.";
                                        objCatalogueRedemptionAlertResponse.ReturnValue = 0;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        objCatalogueRedemptionAlertResponse.ReturnMessage = "[MerchantID is null] or [ObjCatalogueList is null] or [ObjCustShippingAddressDetails is null] or [MerchantMobileNo is null]  or [MerchantEmailID is null] or [UserName is null] ";
                        objCatalogueRedemptionAlertResponse.ReturnValue = 0;
                    }
                }
                else
                {
                    objCatalogueRedemptionAlertResponse.ReturnMessage = "objCatalogueRedemptionAlertRequest is null.";
                    objCatalogueRedemptionAlertResponse.ReturnValue = 0;
                }
            }
            catch (Exception ex)
            {
                objCatalogueRedemptionAlertResponse.ReturnMessage = "Error occoured" + ex.Message;
                objCatalogueRedemptionAlertResponse.ReturnValue = -1;
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " SendCatalogueRedemptionAlertMobileApp() " + ex.Message);
            }
            return objCatalogueRedemptionAlertResponse;
        }

        private string SendSMSAndEmailCatalogueRedemptionAlert(CatalogueRedemptionAlertRequestJson objCatalogueRedemptionAlertRequest, bool IsSMS, bool IsEmail)
        {
            string Response = "";
            try
            {
                MobileAppBO objMobileAppBO = new MobileAppBO();
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.CatalogueRedemptionAlert);
                ObjSmsEmailRetrieveRequest.UserName = objCatalogueRedemptionAlertRequest.UserName;

                if (IsSMS)
                {
                    DateTime IndianActualTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                    ObjSmsEmailRetrieveRequest.Type = UIConstants.SMS;
                    ObjSmsEmailRetrieveRequest.EmailOrMobile = objCatalogueRedemptionAlertRequest.MerchantMobileNo;
                    SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse = objMobileAppBO.GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);

                    for (int i = 0; i < ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count; i++)
                    {

                        List<AlertTemplateDynamicContent> DynamicTemplate = new List<AlertTemplateDynamicContent>(){
                new AlertTemplateDynamicContent ("[LOYALTYID]",objCatalogueRedemptionAlertRequest.UserName),
                new AlertTemplateDynamicContent ("[REDEEMED_POINTS]",objCatalogueRedemptionAlertRequest.TotalPointsRedeemed),
                new AlertTemplateDynamicContent ("[DATE]",IndianActualTime.ToString("dd/MM/yyyy hh:mm tt")),
                new AlertTemplateDynamicContent ("[MEMBER_NAME]",objCatalogueRedemptionAlertRequest.MemberName),
                new AlertTemplateDynamicContent ("[MOBILE_NUMBER]",objCatalogueRedemptionAlertRequest.MerchantMobileNo),
                        new AlertTemplateDynamicContent ("[PRODUCT_NAME]",objCatalogueRedemptionAlertRequest.ObjCatalogueList[0].ProductName)};

                        AlertUtiltityParameters alertUtiltityParameters = new AlertUtiltityParameters
                       (ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i].Type, objCatalogueRedemptionAlertRequest.MerchantMobileNo, DynamicTemplate, ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i]);

                        var Result = SendAlertUtility.SendAlert(alertUtiltityParameters);
                        Response = "1~" + Result;
                    }
                }
                else if (IsEmail)
                {
                    ObjSmsEmailRetrieveRequest.Type = UIConstants.EMAIL;
                    ObjSmsEmailRetrieveRequest.EmailOrMobile = objCatalogueRedemptionAlertRequest.MerchantEmailID;
                    SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse = objMobileAppBO.GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);

                    StringBuilder sbRedemptionDetailsTableBody = new StringBuilder();
                    if (objCatalogueRedemptionAlertRequest.ObjCatalogueList != null)
                    {
                        string sbRedemptionDetailsTableHeader = "<table width=\"100%\" border=\"1\" cellpadding=\"6\" cellspacing=\"0\" style=\"font-family: Calibri\" ><tr bgcolor=\"#DBEAF9\"><td>Redemption Request</td></tr>";
                        sbRedemptionDetailsTableBody.Append(sbRedemptionDetailsTableHeader);
                        int TotalRecords = objCatalogueRedemptionAlertRequest.ObjCatalogueList.Count;
                        int TotalSections = TotalRecords / 4;
                        if ((TotalRecords % 4) > 0)
                            TotalSections++;

                        for (int section = 0; section < TotalSections; section++)
                        {
                            for (int i = 0; i < 6; i++)
                            {
                                sbRedemptionDetailsTableBody.Append("<tr ");
                                if (i == 0)
                                    sbRedemptionDetailsTableBody.Append("bgcolor=\"#87daf0\"><th style=\"width:20%;text-align:left;\">Redemption Reference</th>");
                                if (i == 1)
                                    sbRedemptionDetailsTableBody.Append("><th style=\"width:20%;text-align:left;\">Vendor Name</th>");
                                if (i == 2)
                                    sbRedemptionDetailsTableBody.Append("><th style=\"width:20%;text-align:left;\">Redeemed Points</th>");
                                if (i == 3)
                                    sbRedemptionDetailsTableBody.Append("><th style=\"width:20%;text-align:left;\">Product Code</th>");
                                if (i == 4)
                                    sbRedemptionDetailsTableBody.Append("><th style=\"width:20%;text-align:left;\">Product Name</th>");
                                if (i == 5)
                                    sbRedemptionDetailsTableBody.Append("><th style=\"width:20%;text-align:left;\">Quantity</th>");

                                int loopCount = 0;
                                if (((section + 1) * 4) > TotalRecords)
                                    loopCount = 4 - (((section + 1) * 4) - TotalRecords);
                                else
                                    loopCount = 4;
                                for (int j = 0; j < loopCount; j++)
                                {
                                    if (i == 0)
                                    {
                                        sbRedemptionDetailsTableBody.Append("<td style=\"width:20%;font-weight: bold\">" + objCatalogueRedemptionAlertRequest.ObjCatalogueList[(section * 4) + j].RedemptionRefno + "</td>");
                                    }
                                    if (i == 1)
                                    {
                                        sbRedemptionDetailsTableBody.Append("<td>" + objCatalogueRedemptionAlertRequest.ObjCatalogueList[(section * 4) + j].VendorName + "</td>");
                                    }
                                    if (i == 2)
                                    {
                                        sbRedemptionDetailsTableBody.Append("<td>" + objCatalogueRedemptionAlertRequest.ObjCatalogueList[(section * 4) + j].NoOfPointsDebit + "</td>");
                                    }
                                    if (i == 3)
                                    {
                                        sbRedemptionDetailsTableBody.Append("<td>" + objCatalogueRedemptionAlertRequest.ObjCatalogueList[(section * 4) + j].ProductCode + "</td>");
                                    }
                                    if (i == 4)
                                    {
                                        sbRedemptionDetailsTableBody.Append("<td>" + objCatalogueRedemptionAlertRequest.ObjCatalogueList[(section * 4) + j].ProductName + "</td>");
                                    }
                                    if (i == 5)
                                    {
                                        sbRedemptionDetailsTableBody.Append("<td>" + objCatalogueRedemptionAlertRequest.ObjCatalogueList[(section * 4) + j].NoOfQuantity + "</td>");
                                    }
                                }
                                sbRedemptionDetailsTableBody.Append("</tr>");
                            }
                        }

                        sbRedemptionDetailsTableBody.Append
                         ("<tr><td colspan=\"2\" bgcolor=\"#87daf0\">Shipping Address Details</td></tr><tr><td>Deliver to</td><td>"
                           + UICommon.InitCapEveryword(objCatalogueRedemptionAlertRequest.ObjCustShippingAddressDetails.FullName) + "</td></tr><tr><td style=\"width:50%\">Address</td><td>"
                             + UICommon.InitCap(objCatalogueRedemptionAlertRequest.ObjCustShippingAddressDetails.Address1) + "</td></tr><tr><td style=\"width:50%\">Zip Code</td><td style=\"width:50%\">"
                              + objCatalogueRedemptionAlertRequest.ObjCustShippingAddressDetails.Zip + "</td></tr><tr><td style=\"width:50%\">City</td><td style=\"width:50%\">"
                               + UICommon.InitCap(objCatalogueRedemptionAlertRequest.ObjCustShippingAddressDetails.CityName) + "</td></tr><tr><td style=\"width:50%\">State</td><td style=\"width:50%\">"
                                 + objCatalogueRedemptionAlertRequest.ObjCustShippingAddressDetails.StateName + "</td></tr><tr><td style=\"width:50%\">Contact </td><td style=\"width:50%\">"
                         + objCatalogueRedemptionAlertRequest.ObjCustShippingAddressDetails.Mobile + "</td></tr>"
                          );
                        sbRedemptionDetailsTableBody.Append("</table>");
                    }

                    for (int i = 0; i < ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count; i++)
                    {
                        List<AlertTemplateDynamicContent> DynamicTemplate = new List<AlertTemplateDynamicContent>()
                    {
                    new AlertTemplateDynamicContent("[LOYALTY_ID]", objCatalogueRedemptionAlertRequest.UserName),
                    new AlertTemplateDynamicContent("[MEMBER_NAME]", objCatalogueRedemptionAlertRequest.MemberName),
                    new AlertTemplateDynamicContent("[TABLE]",Convert.ToString(sbRedemptionDetailsTableBody))
                    };

                        //Sending E-Mail
                        AlertUtiltityParameters alertUtiltityParametersMail = new AlertUtiltityParameters
                                      (UIConstants.EMAIL, objCatalogueRedemptionAlertRequest.MerchantEmailID, DynamicTemplate, ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i]);
                        // SMS will not send throgh thread
                        alertUtiltityParametersMail.ShouldUseThreading = false;
                        alertUtiltityParametersMail.MailHeaderText = UIConstants.REDEMPTION_HEADER;
                        var MailResult = SendAlertUtility.SendAlert(alertUtiltityParametersMail);
                        Response = "1~" + MailResult;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "SendSMSAndEmailCatalogueRedemptionAlert()" + ex.Message);
                Response = "-1~Error Occoured In :SendSMSAndEmailCatalogueRedemptionAlert() ;" + ex.Message;
            }
            return Response;
        }

        #endregion

        public SaveOrderRequestRetrieveResponse SendProductRequestMail(SaveOrderRequestRetrieveRequest objSaveOrderRequestRetrieveRequest)
        {

            SaveOrderRequestRetrieveResponse objSaveOrderRequestRetrieveResponse = new SaveOrderRequestRetrieveResponse();
            try
            {
                int Quantity = objSaveOrderRequestRetrieveRequest.Quantity;
                string OrderNo = objSaveOrderRequestRetrieveRequest.OrderNo,
                       CustomerPrefix = objSaveOrderRequestRetrieveRequest.CustomerPrefix,
                       CustomerName = objSaveOrderRequestRetrieveRequest.CustomerName,
                       MembershipID = objSaveOrderRequestRetrieveRequest.MembershipID,
                       CustomerEMail = objSaveOrderRequestRetrieveRequest.CustomerEMail,
                       MerchantUserName = objSaveOrderRequestRetrieveRequest.MerchantUserName;

                //send Conformation Mail to customer
                SendConformationMailToCustomer(OrderNo, Quantity, CustomerName, MerchantUserName, CustomerEMail);
                //send Mail to admin 
                SendOrderRequestToAdmin(OrderNo, Quantity, CustomerPrefix, CustomerName, MembershipID, CustomerEMail, MerchantUserName);
                objSaveOrderRequestRetrieveResponse.ReturnValue = 1;

            }
            catch (Exception ex)
            {
                objSaveOrderRequestRetrieveResponse.ReturnValue = -1;
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "SendProductRequestMail()" + ex.Message);
            }

            return objSaveOrderRequestRetrieveResponse;

        }

        public void SendConformationMailToCustomer(string OrderNo, int Quantity, string CustomerName, string MerchantUserName, string CustomerEMail)
        {
            try
            {
                string Subject = "Order Conformation";

                StringBuilder objStringBuilder = new StringBuilder();
                objStringBuilder.Append("<html><head></head><body>");
                objStringBuilder.Append("<span class='style1'>Dear " + CustomerName + ",</span> <br>");
                objStringBuilder.Append("<br />");
                objStringBuilder.Append("<span class='style1'>Greetings!</span> <br>");
                objStringBuilder.Append("<br />");
                objStringBuilder.Append("<span class='style2' style='line-height:1.5em;'>Your order has been placed and mail has been sent to admin for further process.</span> <br>");
                objStringBuilder.Append("<br />");
                objStringBuilder.Append("<span class='style2' style='line-height:1.5em;'><b>Order Details:</b></span> <br>");
                objStringBuilder.Append("<br />");
                objStringBuilder.Append("<span class='style2' style='line-height:1.5em;'>Battery Size: " + OrderNo + "</span> <br>");
                objStringBuilder.Append("<span class='style2' style='line-height:1.5em;'>Quantity: " + Quantity + "</span> <br>");
                objStringBuilder.Append("<br />");
                objStringBuilder.Append("<span class='style2' style='line-height:1.5em;'> Regards,");
                objStringBuilder.Append("<br/>Ear Science Centre");
                objStringBuilder.Append("</body></html>");

                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse = new SmsEmailRetrieveResponse();
                ObjSmsEmailRetrieveResponse.lstSmsEmailDetails = new List<SmsEmail>();
                List<AlertTemplateDynamicContent> DynamicTemplate = new List<AlertTemplateDynamicContent>();
                SmsEmail objSmsEmail = new SmsEmail();
                objSmsEmail.Template = objStringBuilder.ToString();
                objSmsEmail.Subject = Subject;

                ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Add(objSmsEmail);
                for (int i = 0; i < ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count; i++)
                {

                    AlertUtiltityParameters alertUtiltityParameters = new AlertUtiltityParameters
                 (UIConstants.EMAIL, CustomerEMail, DynamicTemplate, ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i]);

                    List<string> smtpdetailslst = GetSmtp(MerchantUserName);
                    alertUtiltityParameters.MailHeaderText = UIConstants.OTHERS_HEADER;
                    alertUtiltityParameters.ServerDetails = new Dictionary<string, string>
                {
                    {smtpdetailslst[0], "SMTPAddress"},
                    {smtpdetailslst[1], "SMTPUserName"},
                    {smtpdetailslst[2], "SMTPPassword"},
                    {smtpdetailslst[3], "FromEmail"}
                };

                    alertUtiltityParameters.ShouldUseThreading = false;
                    alertUtiltityParameters.MailHeaderText = UIConstants.OTHERS_HEADER;
                    var AlertResult = SendAlertUtility.SendAlert(alertUtiltityParameters);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "SendConformationMailToCustomer()" + ex.Message);

            }
        }

        public void SendOrderRequestToAdmin(string OrderNo, int Quantity, string CustomerPrefix, string CustomerName, string MembershipID, string CustomerEMail, string MerchantUserName)
        {
            try
            {
                string Subject = "Order Request";

                StringBuilder objStringBuilder = new StringBuilder();
                objStringBuilder.Append("<html><head></head><body>");
                objStringBuilder.Append("<span class='style1'>Dear Admin,</span> <br>");
                objStringBuilder.Append("<br />");
                objStringBuilder.Append("<span class='style1'>Greetings!</span> <br>");
                objStringBuilder.Append("<br />");
                objStringBuilder.Append("<span class='style2' style='line-height:1.5em;'>" + CustomerPrefix + CustomerName + ", Membership ID: " + MembershipID + " has sent you order request.</span> <br>");
                objStringBuilder.Append("<br />");
                objStringBuilder.Append("<span class='style2' style='line-height:1.5em;'><b>Order Details:</b></span> <br>");
                objStringBuilder.Append("<br />");
                objStringBuilder.Append("<span class='style2' style='line-height:1.5em;'>Battery Size: " + OrderNo + "</span> <br>");
                objStringBuilder.Append("<span class='style2' style='line-height:1.5em;'>Quantity: " + Quantity + "</span> <br>");
                objStringBuilder.Append("<br />");
                objStringBuilder.Append("<span class='style2' style='line-height:1.5em;'> Regards,");
                objStringBuilder.Append("<br/>" + CustomerName);
                objStringBuilder.Append("</body></html>");

                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse = new SmsEmailRetrieveResponse();
                ObjSmsEmailRetrieveResponse.lstSmsEmailDetails = new List<SmsEmail>();
                List<AlertTemplateDynamicContent> DynamicTemplate = new List<AlertTemplateDynamicContent>();
                SmsEmail objSmsEmail = new SmsEmail();
                objSmsEmail.Template = objStringBuilder.ToString();
                objSmsEmail.Subject = Subject;

                ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Add(objSmsEmail);

                for (int i = 0; i < ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count; i++)
                {

                    AlertUtiltityParameters alertUtiltityParameters = new AlertUtiltityParameters
                 (UIConstants.EMAIL, MERCHANT_EMAIL_ID, DynamicTemplate, ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i]);

                    List<string> smtpdetailslst = GetSmtp(MerchantUserName);
                    alertUtiltityParameters.MailHeaderText = UIConstants.OTHERS_HEADER;
                    alertUtiltityParameters.ServerDetails = new Dictionary<string, string>
                {
                    {smtpdetailslst[0], "SMTPAddress"},
                    {smtpdetailslst[1], "SMTPUserName"},
                    {smtpdetailslst[2], "SMTPPassword"},
                    {smtpdetailslst[3], "FromEmail"}
                };

                    alertUtiltityParameters.ShouldUseThreading = false;
                    var AlertResult = SendAlertUtility.SendAlert(alertUtiltityParameters);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "SendOrderRequestToAdmin()" + ex.Message);
            }
        }

        public bool CheckAllExistancy(LocationSaveRequest ObjLocationSaveRequest)
        {
            bool returnValue = false;
            try
            {

                LocationSaveResponse ObjLocationSaveResponse = mobileAppBO.CheckUserNameExists(ObjLocationSaveRequest);
                if (ObjLocationSaveResponse.ReturnValue == 1)
                    returnValue = true;
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "CheckAllExistancy()::" + ex.Message);
            }
            return returnValue;
        }

        public LocationSaveResponse CheckUserNameExists(LocationSaveRequest ObjLocationSaveRequest)
        {
            LocationSaveResponse ObjLocationSaveResponse = null;
            try
            {
                ObjLocationSaveResponse = mobileAppBO.CheckUserNameExists(ObjLocationSaveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "CheckAllExistancy()::" + ex.Message);
            }
            return ObjLocationSaveResponse;
        }

        public LookUpRetrieveResponse GetLookUpDetails(LookUpRetrieveRequest ObjLookUpRetrieveRequest)
        {
            LookUpRetrieveResponse LookUpRetrieveResponse = null;
            try
            {
                LookUpRetrieveResponse = mobileAppBO.GetLookUpDetails(ObjLookUpRetrieveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "GetLookUpDetails()" + ex.Message);
            }
            return LookUpRetrieveResponse;
        }

        public BillingSaveResponse SaveInvoiceDetailsChawla(BillingSaveRequest BillingSaveRequest)
        {
            BillingSaveResponse BillingSaveResponse = null;
            try
            {
                BillingSaveResponse = mobileAppBO.SaveInvoiceDetailsChawla(BillingSaveRequest);
                if (BillingSaveResponse != null)
                {
                    SendPointsAccumulationSms(MERCHANT_USERNAME, BillingSaveResponse.FirstName, BillingSaveResponse.Mobile
                                                , Convert.ToString(BillingSaveResponse.CreditedPoints), Convert.ToString(BillingSaveResponse.PointsBalance));
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "SaveInvoiceDetailsChawla()" + ex.Message);
            }
            return BillingSaveResponse;
        }

        public CustomerRetrieveResponse GetCustomerDetailsByID(CustomerRetrieveRequest ObjCustomerRetrieveRequest)
        {
            CustomerRetrieveResponse objCustomerRetrieveResponse = null;
            try
            {
                objCustomerRetrieveResponse = mobileAppBO.GetCustomerDetailsByID(ObjCustomerRetrieveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "GetCustomerDetailsByID : " + ex.Message);
            }
            return objCustomerRetrieveResponse;
        }

        #region For Booking Appointment

        /// <summary>
        /// This method will send email and sms to location and merchant once any customer raise an appointment request.
        /// </summary>
        public AppointmentResponse CustomerAppointmentMobileApp(AppointmentRequest objAppointmentRequest)
        {
            AppointmentResponse objAppointmentResponse = new AppointmentResponse();
            try
            {
                bool IsValid = false;
                if (objAppointmentRequest != null)
                {
                    // Checking customer name.
                    IsValid = objAppointmentRequest.CustomerName == (string.Empty) || objAppointmentRequest.CustomerName == null ? false : true;
                    if (!IsValid)
                    {
                        objAppointmentResponse.ReturnMessage = "Customer name should not be empty.";
                        objAppointmentResponse.ReturnValue = -2;
                        return objAppointmentResponse;
                    }

                    // Checking the mobile number.
                    IsValid = objAppointmentRequest.MobileNo == (string.Empty) || objAppointmentRequest.MobileNo == null ? false : true;
                    if (!IsValid)
                    {
                        objAppointmentResponse.ReturnMessage = "Mobile Number should not be empty.";
                        objAppointmentResponse.ReturnValue = -2;
                        return objAppointmentResponse;
                    }

                    // Checking the mobile number.
                    IsValid = objAppointmentRequest.LocationID == 0 || objAppointmentRequest.LocationID == null ? false : true;
                    if (!IsValid)
                    {
                        objAppointmentResponse.ReturnMessage = "Please select Location.";
                        objAppointmentResponse.ReturnValue = -2;
                        return objAppointmentResponse;
                    }

                    objAppointmentResponse = mobileAppBO.CustomerAppointmentMobileApp(objAppointmentRequest);
                    if (!string.IsNullOrEmpty(objAppointmentResponse.ReturnMessage))
                    {
                        string[] arg = objAppointmentResponse.ReturnMessage.ToString().Split('~');

                        switch (arg[0])
                        {
                            // Success
                            case UIConstants.VALUE_ONE:
                                {
                                    SendEmailAndSMSToMerchant(objAppointmentRequest, objAppointmentResponse);
                                    objAppointmentResponse.LocationEmail = LOCATION_EMAILID;
                                    objAppointmentResponse.LocationMob = LOCATION_MOBILE_NUM;
                                    SendEmailAndSMSToLocation(objAppointmentRequest, objAppointmentResponse);
                                    objAppointmentResponse.ReturnMessage = arg[1];
                                    objAppointmentResponse.ReturnValue = 1;
                                    break;
                                }

                            // Failed with exception
                            case UIConstants.VALUE_MINUS_ONE:
                                {
                                    objAppointmentResponse.ReturnMessage = arg[1];
                                    objAppointmentResponse.ReturnValue = -1;
                                    break;
                                }

                            // Failed with proper validation.
                            case UIConstants.VALUE_MINUS_TWO:
                                {
                                    objAppointmentResponse.ReturnMessage = arg[1];
                                    objAppointmentResponse.ReturnValue = -2;
                                    break;
                                }

                            // Worst Case.
                            default:
                                {
                                    ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "CustomerAppointmentMobileApp() :: default case executing");
                                    objAppointmentResponse.ReturnMessage = "default case executing";
                                    objAppointmentResponse.ReturnValue = -1;
                                    break;
                                }
                        }
                    }
                    else
                    {
                        objAppointmentResponse.ReturnMessage = "ReturnMessage is empty";
                        objAppointmentResponse.ReturnValue = -1;
                        ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "CustomerAppointmentMobileApp() :: ReturnMessage is empty");
                    }
                }
                else
                {
                    objAppointmentResponse.ReturnMessage = "Appointment Request is null";
                    objAppointmentResponse.ReturnValue = -1;
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "CustomerAppointmentMobileApp : " + ex.Message);
            }
            return objAppointmentResponse;
        }

        /// <summary>
        /// For sending Email and SMS To Location for an appointment
        /// </summary>
        private void SendEmailAndSMSToLocation(AppointmentRequest objAppointmentRequest, AppointmentResponse objAppointmentResponse)
        {
            try
            {

                SmsEmail ObjSmsEmail = new SmsEmail();
                MobileAppBO objMobileAppBO = new MobileAppBO();
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse_Email = null;
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse_Sms = null;
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.AppointmentAlertForLocation);
                ObjSmsEmailRetrieveRequest.Type = UIConstants.SMS;
                ObjSmsEmailRetrieveRequest.UserName = objAppointmentResponse.MerchantUsername;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = objAppointmentResponse.LocationMob;
                ObjSmsEmailRetrieveResponse_Sms = objMobileAppBO.GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);

                ObjSmsEmailRetrieveRequest.Type = UIConstants.EMAIL;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = objAppointmentResponse.LocationEmail;
                ObjSmsEmailRetrieveResponse_Email = objMobileAppBO.GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);

                StringBuilder sbServiceType = new StringBuilder();
                string ServiceType = string.Empty;
                if (objAppointmentRequest.objAppointmentServiceList.Count > 0)
                {
                    sbServiceType.Append("<ul>");

                    //sbServiceType.Append(objAppointmentRequest.objAppointmentServiceList[i] + "</li>");
                    if (!string.IsNullOrEmpty(objAppointmentRequest.objAppointmentServiceList[0].Accident))
                        sbServiceType.Append("<li>" + objAppointmentRequest.objAppointmentServiceList[0].Accident + "</li>");
                    //For Toyota Millennium(Added by Nagendra)
                    if (!string.IsNullOrEmpty(objAppointmentRequest.objAppointmentServiceList[0].PeriodicServices))
                        sbServiceType.Append("<li>" + objAppointmentRequest.objAppointmentServiceList[0].PeriodicServices + "</li>");

                    if (!string.IsNullOrEmpty(objAppointmentRequest.objAppointmentServiceList[0].BreakDown))
                        sbServiceType.Append("<li>" + objAppointmentRequest.objAppointmentServiceList[0].BreakDown + "</li>");

                    if (!string.IsNullOrEmpty(objAppointmentRequest.objAppointmentServiceList[0].BodyPaintRepairs))
                        sbServiceType.Append("<li>" + objAppointmentRequest.objAppointmentServiceList[0].BodyPaintRepairs + "</li>");

                    if (!string.IsNullOrEmpty(objAppointmentRequest.objAppointmentServiceList[0].ExtendedWarranty))
                        sbServiceType.Append("<li>" + objAppointmentRequest.objAppointmentServiceList[0].ExtendedWarranty + "</li>");

                    if (!string.IsNullOrEmpty(objAppointmentRequest.objAppointmentServiceList[0].FreeService))
                        sbServiceType.Append("<li>" + objAppointmentRequest.objAppointmentServiceList[0].FreeService + "</li>");

                    if (!string.IsNullOrEmpty(objAppointmentRequest.objAppointmentServiceList[0].InsuranceRenewal))
                        sbServiceType.Append("<li>" + objAppointmentRequest.objAppointmentServiceList[0].InsuranceRenewal + "</li>");

                    if (!string.IsNullOrEmpty(objAppointmentRequest.objAppointmentServiceList[0].UsedCarValuation))
                        sbServiceType.Append("<li>" + objAppointmentRequest.objAppointmentServiceList[0].UsedCarValuation + "</li>");

                    if (!string.IsNullOrEmpty(objAppointmentRequest.objAppointmentServiceList[0].TestDrive))
                        sbServiceType.Append("<li>" + objAppointmentRequest.objAppointmentServiceList[0].TestDrive + "</li>");

                    if (!string.IsNullOrEmpty(objAppointmentRequest.objAppointmentServiceList[0].PaidService))
                        sbServiceType.Append("<li>" + objAppointmentRequest.objAppointmentServiceList[0].PaidService + "</li>");

                    if (!string.IsNullOrEmpty(objAppointmentRequest.objAppointmentServiceList[0].RepairJob))
                        sbServiceType.Append("<li>" + objAppointmentRequest.objAppointmentServiceList[0].RepairJob + "</li>");

                    if (!string.IsNullOrEmpty(objAppointmentRequest.objAppointmentServiceList[0].others))
                        sbServiceType.Append("<li>" + objAppointmentRequest.objAppointmentServiceList[0].TestDrive + "</li>");

                    if (!string.IsNullOrEmpty(objAppointmentRequest.objAppointmentServiceList[0].others))
                        sbServiceType.Append("<li>" + objAppointmentRequest.objAppointmentServiceList[0].others + "</li>");

                    sbServiceType.Append("</ul>");
                    ServiceType = sbServiceType.ToString();
                }

                List<AlertTemplateDynamicContent> DynamicTemplate = new List<AlertTemplateDynamicContent>()
                {
                    new AlertTemplateDynamicContent("[Customer name]",objAppointmentRequest.CustomerName),
                    new AlertTemplateDynamicContent("[date]",Convert.ToString(UICommon.MMDDtoDDMM(objAppointmentRequest.AppointmentDate.ToString()))),
                    new AlertTemplateDynamicContent("[MobileNo]",objAppointmentRequest.MobileNo),
                    new AlertTemplateDynamicContent("[VehicleModel]",objAppointmentRequest.VehicleModel),
                    new AlertTemplateDynamicContent("[location name]",objAppointmentResponse.LocationUsername),
                    new AlertTemplateDynamicContent("[Comments]",objAppointmentRequest.Comments),
                    new AlertTemplateDynamicContent("[ServiceType]",ServiceType = ServiceType != string.Empty? ServiceType : "--")
                };

                //Sending E-Mail
                for (int i = 0; i < ObjSmsEmailRetrieveResponse_Email.lstSmsEmailDetails.Count; i++)
                {
                    AlertUtiltityParameters alertUtiltityParametersMail = new AlertUtiltityParameters
                                  (UIConstants.EMAIL, objAppointmentResponse.LocationEmail, DynamicTemplate, ObjSmsEmailRetrieveResponse_Email.lstSmsEmailDetails[i]);
                    alertUtiltityParametersMail.ShouldUseThreading = false;
                    alertUtiltityParametersMail.MailHeaderText = UIConstants.OTHERS_HEADER;
                    var MailResult = SendAlertUtility.SendAlert(alertUtiltityParametersMail);
                }

                //Sending SMS
                for (int i = 0; i < ObjSmsEmailRetrieveResponse_Sms.lstSmsEmailDetails.Count; i++)
                {
                    AlertUtiltityParameters alertUtiltityParametersSMS = new AlertUtiltityParameters
                                  (ObjSmsEmailRetrieveResponse_Sms.lstSmsEmailDetails[i].Type, objAppointmentResponse.LocationMob, DynamicTemplate, ObjSmsEmailRetrieveResponse_Sms.lstSmsEmailDetails[i]);
                    var SMSResult = SendAlertUtility.SendAlert(alertUtiltityParametersSMS);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "SendEmailAndSMSToLocation : " + ex.Message);
            }
        }

        /// <summary>
        /// For sending Email and SMS To Merchant for an appointment
        /// </summary>
        private void SendEmailAndSMSToMerchant(AppointmentRequest objAppointmentRequest, AppointmentResponse objAppointmentResponse)
        {
            try
            {

                SmsEmail ObjSmsEmail = new SmsEmail();
                MobileAppBO objMobileAppBO = new MobileAppBO();
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse_Email = null;
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse_Sms = null;
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.AppointmentAlertForMerchant);
                ObjSmsEmailRetrieveRequest.Type = UIConstants.SMS;
                ObjSmsEmailRetrieveRequest.UserName = objAppointmentResponse.MerchantUsername;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = objAppointmentResponse.MerchantMob;
                ObjSmsEmailRetrieveResponse_Sms = objMobileAppBO.GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);

                ObjSmsEmailRetrieveRequest.Type = UIConstants.EMAIL;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = objAppointmentResponse.MerchantEmail;
                ObjSmsEmailRetrieveResponse_Email = objMobileAppBO.GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);

                StringBuilder sbServiceType = new StringBuilder();
                string ServiceType = string.Empty;
                if (objAppointmentRequest.objAppointmentServiceList.Count > 0)
                {

                    sbServiceType.Append("<ul>");

                    //sbServiceType.Append(objAppointmentRequest.objAppointmentServiceList[i] + "</li>");
                    if (!string.IsNullOrEmpty(objAppointmentRequest.objAppointmentServiceList[0].Accident))
                        sbServiceType.Append("<li>" + objAppointmentRequest.objAppointmentServiceList[0].Accident + "</li>");
                    if (!string.IsNullOrEmpty(objAppointmentRequest.objAppointmentServiceList[0].BreakDown))
                        sbServiceType.Append("<li>" + objAppointmentRequest.objAppointmentServiceList[0].BreakDown + "</li>");
                    if (!string.IsNullOrEmpty(objAppointmentRequest.objAppointmentServiceList[0].FreeService))
                        sbServiceType.Append("<li>" + objAppointmentRequest.objAppointmentServiceList[0].FreeService + "</li>");
                    if (!string.IsNullOrEmpty(objAppointmentRequest.objAppointmentServiceList[0].InsuranceRenewal))
                        sbServiceType.Append("<li>" + objAppointmentRequest.objAppointmentServiceList[0].InsuranceRenewal + "</li>");
                    if (!string.IsNullOrEmpty(objAppointmentRequest.objAppointmentServiceList[0].PaidService))
                        sbServiceType.Append("<li>" + objAppointmentRequest.objAppointmentServiceList[0].PaidService + "</li>");
                    if (!string.IsNullOrEmpty(objAppointmentRequest.objAppointmentServiceList[0].RepairJob))
                        sbServiceType.Append("<li>" + objAppointmentRequest.objAppointmentServiceList[0].RepairJob + "</li>");
                    if (!string.IsNullOrEmpty(objAppointmentRequest.objAppointmentServiceList[0].others))
                        sbServiceType.Append("<li>" + objAppointmentRequest.objAppointmentServiceList[0].others + "</li>");


                    sbServiceType.Append("</ul>");
                    ServiceType = sbServiceType.ToString();
                }

                List<AlertTemplateDynamicContent> DynamicTemplate = new List<AlertTemplateDynamicContent>()
                {
                    new AlertTemplateDynamicContent("[Customer name]",objAppointmentRequest.CustomerName),
                    new AlertTemplateDynamicContent("[date]",Convert.ToString(UICommon.MMDDtoDDMM(objAppointmentRequest.AppointmentDate.ToString()))),
                    new AlertTemplateDynamicContent("[MobileNo]",objAppointmentRequest.MobileNo),
                    new AlertTemplateDynamicContent("[VehicleModel]",objAppointmentRequest.VehicleModel),
                    new AlertTemplateDynamicContent("[location name]",objAppointmentResponse.LocationUsername),
                    new AlertTemplateDynamicContent("[Comments]",objAppointmentRequest.Comments),
                    new AlertTemplateDynamicContent("[ServiceType]",ServiceType = ServiceType != string.Empty? ServiceType : "--")

                };

                //Sending E-Mail

                for (int i = 0; i < ObjSmsEmailRetrieveResponse_Email.lstSmsEmailDetails.Count; i++)
                {
                    AlertUtiltityParameters alertUtiltityParametersMail = new AlertUtiltityParameters
                                  (UIConstants.EMAIL, objAppointmentResponse.MerchantEmail, DynamicTemplate, ObjSmsEmailRetrieveResponse_Email.lstSmsEmailDetails[i]);
                    alertUtiltityParametersMail.ShouldUseThreading = false;
                    alertUtiltityParametersMail.MailHeaderText = UIConstants.OTHERS_HEADER;
                    var MailResult = SendAlertUtility.SendAlert(alertUtiltityParametersMail);
                }
                //Sending SMS

                for (int j = 0; j < ObjSmsEmailRetrieveResponse_Sms.lstSmsEmailDetails.Count; j++)
                {
                    AlertUtiltityParameters alertUtiltityParametersSMS = new AlertUtiltityParameters
                                  (ObjSmsEmailRetrieveResponse_Sms.lstSmsEmailDetails[j].Type, objAppointmentResponse.MerchantMob, DynamicTemplate, ObjSmsEmailRetrieveResponse_Sms.lstSmsEmailDetails[j]);
                    var SMSResult = SendAlertUtility.SendAlert(alertUtiltityParametersSMS);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "SendEmailAndSMSToMerchant : " + ex.Message);
            }
        }

        #endregion For Booking Appointment

        #region encash
        public CustomerEncashRedemptionRetrieveResponse SaveCustomerEncashRedemptionDetails(CustomerEncashRedemptionRetrieveRequest objCustomerEncashRedemptionRetrieveRequest)
        {
            CustomerEncashRedemptionRetrieveResponse objCustomerEncashRedemptionRetrieveResponse = null;
            try
            {
                objCustomerEncashRedemptionRetrieveRequest.IsMultiMerchant = false;
                objCustomerEncashRedemptionRetrieveResponse = mobileAppBO.SaveCustomerEncashRedemptionDetails(objCustomerEncashRedemptionRetrieveRequest);
                if (objCustomerEncashRedemptionRetrieveResponse.objCustomerBalanceInfo != null)
                {

                    int PointsBalance; string Name, LocationName, PointsDebited, Mobile, Email, Invoice;
                    PointsBalance = objCustomerEncashRedemptionRetrieveResponse.objCustomerBalanceInfo.PointsBalance;
                    Name = objCustomerEncashRedemptionRetrieveResponse.objCustomerBalanceInfo.FirstName + " " + objCustomerEncashRedemptionRetrieveResponse.objCustomerBalanceInfo.LastName;
                    LocationName = objCustomerEncashRedemptionRetrieveResponse.objCustomerBalanceInfo.LocationName;
                    PointsDebited = objCustomerEncashRedemptionRetrieveResponse.objCustomerBalanceInfo.PointsDebited;
                    Mobile = objCustomerEncashRedemptionRetrieveResponse.objCustomerBalanceInfo.Mobile;
                    Email = objCustomerEncashRedemptionRetrieveResponse.objCustomerBalanceInfo.Email;
                    Invoice = Convert.ToString(objCustomerEncashRedemptionRetrieveResponse.objCustomerBalanceInfo.AwardRewardId);

                    sendRedemptionSms(objCustomerEncashRedemptionRetrieveRequest.objCustomerBalanceInfo.LoyaltyId, Name, PointsDebited, Convert.ToString(PointsBalance), Mobile);
                }
            }
            catch (Exception ex)
            {

                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "SaveCustomerEncashRedemptionDetails() : " + ex.Message);
            }
            return objCustomerEncashRedemptionRetrieveResponse;

        }

        #region encash email & sms


        private void sendRedemptionSms(string Username, string CustomerName, string PointsDebited, string PointsBalance, string Mobile)
        {
            try
            {
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.PointDebitFromLocation_EXE);
                ObjSmsEmailRetrieveRequest.Type = UIConstants.SMS;
                ObjSmsEmailRetrieveRequest.UserName = Username;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = Mobile;
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponseSMS = mobileAppBO.GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);
                for (int i = 0; i < ObjSmsEmailRetrieveResponseSMS.lstSmsEmailDetails.Count; i++)
                {

                    List<AlertTemplateDynamicContent> DynamicTemplate = new List<AlertTemplateDynamicContent>(){
                new AlertTemplateDynamicContent (CLIENT_NAME,CustomerName),
                new AlertTemplateDynamicContent (POINTS,PointsDebited),
                new AlertTemplateDynamicContent (BALANCE,PointsBalance)};

                    AlertUtiltityParameters alertUtiltityParameters = new AlertUtiltityParameters
                   (ObjSmsEmailRetrieveResponseSMS.lstSmsEmailDetails[i].Type, Mobile, DynamicTemplate, ObjSmsEmailRetrieveResponseSMS.lstSmsEmailDetails[i]);
                    alertUtiltityParameters.IsGetRequest = true;

                    var Result = SendAlertUtility.SendAlert(alertUtiltityParameters);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "sendRedemptionSms" + ex.Message);
            }
        }


        #endregion encash email & sms
        #endregion encash

        #region SMS & Email
        //For customer registration sms
        private void SendCustRegCredentials(string custUserName, string CustomerName, string Password, string Mobile, string LoyaltyId)
        {
            #region sms
            string Result = string.Empty;
            try
            {
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.MembershipLoginCredentials);
                ObjSmsEmailRetrieveRequest.Type = UIConstants.SMS;
                ObjSmsEmailRetrieveRequest.UserName = MERCHANT_USERNAME;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = Mobile;
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponseSMS = mobileAppBO.GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);
                if (ObjSmsEmailRetrieveResponseSMS.lstSmsEmailDetails.Count > 0)
                {

                    for (int i = 0; i < ObjSmsEmailRetrieveResponseSMS.lstSmsEmailDetails.Count; i++)
                    {

                        List<AlertTemplateDynamicContent> DynamicTemplate = new List<AlertTemplateDynamicContent>() {
                      new AlertTemplateDynamicContent("[Client name]", CustomerName),
                      new AlertTemplateDynamicContent("[membership id]", LoyaltyId),
                      new AlertTemplateDynamicContent("[Pin]", Password)
                     };

                        AlertUtiltityParameters alertUtiltityParameters = new AlertUtiltityParameters
                                      (ObjSmsEmailRetrieveResponseSMS.lstSmsEmailDetails[i].Type, Mobile, DynamicTemplate, ObjSmsEmailRetrieveResponseSMS.lstSmsEmailDetails[i]);
                        alertUtiltityParameters.IsGetRequest = true;
                        if (DOMAIN == UIConstants.REDINGTON)
                            alertUtiltityParameters.VendorMethodName = SMSVendorMethodEnum.SmsCountry;
                        Result = SendAlertUtility.SendAlert(alertUtiltityParameters);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " : SendSms " + ex.Message);
            }
            #endregion sms

        }

        //For customer registration sms
        private void SendCustRegCredentialsEmail(string custUserName, string CustomerName, string Password, string Email, string LoyaltyId)
        {
            #region Email
            string Result = string.Empty;
            try
            {
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.MembershipLoginCredentials);
                ObjSmsEmailRetrieveRequest.Type = UIConstants.EMAIL;
                ObjSmsEmailRetrieveRequest.UserName = MERCHANT_USERNAME;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = Email;
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponseSMS = mobileAppBO.GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);
                if (ObjSmsEmailRetrieveResponseSMS.lstSmsEmailDetails.Count > 0)
                {
                    for (int i = 0; i < ObjSmsEmailRetrieveResponseSMS.lstSmsEmailDetails.Count; i++)
                    {

                        List<AlertTemplateDynamicContent> DynamicTemplate = new List<AlertTemplateDynamicContent>() {
                      new AlertTemplateDynamicContent("[Client name]", CustomerName),
                      new AlertTemplateDynamicContent("[membership id]", LoyaltyId),
                      new AlertTemplateDynamicContent("[Pin]", Password)
                     };

                        AlertUtiltityParameters alertUtiltityParameters = new AlertUtiltityParameters
                                      (UIConstants.EMAIL, Email, DynamicTemplate, ObjSmsEmailRetrieveResponseSMS.lstSmsEmailDetails[i]);
                        alertUtiltityParameters.IsGetRequest = true;
                        alertUtiltityParameters.MailHeaderText = UIConstants.WELCOME_HEADER;
                        Result = SendAlertUtility.SendAlert(alertUtiltityParameters);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " : SendSms " + ex.Message);
            }
            #endregion sms
        }

        private void SendPointsAccumulationSms(string MerchantUserName, string CustomerName, string Mobile, string CreditPoint, string CustomerPointBalance)
        {
            try
            {
                SmsEmail ObjSmsEmail = new SmsEmail();
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.PointsAccumulation);
                ObjSmsEmailRetrieveRequest.UserName = MerchantUserName;

                //SMS
                ObjSmsEmailRetrieveRequest.Type = UIConstants.SMS;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = Mobile;
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse_SMS = mobileAppBO.GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);
                if (ObjSmsEmailRetrieveResponse_SMS.lstSmsEmailDetails.Count > 0)
                {
                    for (int i = 0; i < ObjSmsEmailRetrieveResponse_SMS.lstSmsEmailDetails.Count; i++)
                    {
                        List<AlertTemplateDynamicContent> DynamicTemplate = new List<AlertTemplateDynamicContent>()
                            {
                            new AlertTemplateDynamicContent("[Client name]", CustomerName),
                            new AlertTemplateDynamicContent("[Noofpoints]", CreditPoint),
                            new AlertTemplateDynamicContent("[BALANCE]", CustomerPointBalance)
                            };

                        //Sending SMS
                        AlertUtiltityParameters alertUtiltityParametersSMS = new AlertUtiltityParameters
                                      (ObjSmsEmailRetrieveResponse_SMS.lstSmsEmailDetails[i].Type, Mobile, DynamicTemplate, ObjSmsEmailRetrieveResponse_SMS.lstSmsEmailDetails[i]);

                        alertUtiltityParametersSMS.IsGetRequest = true;
                        var SMSResult = SendAlertUtility.SendAlert(alertUtiltityParametersSMS);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + ":: SendEmailSms() " + ex.Message);
            }
        }

        private void SendOtpInEmail(UserRetrieveRequest_Loyalty ObjUserRetrieveRequest)
        {
            try
            {
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();

                if (ObjUserRetrieveRequest.OTPType == "Enrollment")
                    ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.OTPForEnrollmentAuthentication);
                else
                    ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.OTPForRewardCardsENCashAuthorization);

                ObjSmsEmailRetrieveRequest.Type = UIConstants.EMAIL;
                ObjSmsEmailRetrieveRequest.UserName = ObjUserRetrieveRequest.MerchantUserName;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = ObjUserRetrieveRequest.MobileNo;
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponseEmail = mobileAppBO.GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);
                for (int i = 0; i < ObjSmsEmailRetrieveResponseEmail.lstSmsEmailDetails.Count; i++)
                {
                    List<AlertTemplateDynamicContent> DynamicTemplate = new List<AlertTemplateDynamicContent>(){
                new AlertTemplateDynamicContent (CUSTOMER_NAME,ObjUserRetrieveRequest.Name),
                new AlertTemplateDynamicContent (AUTHRIZEDNO, ObjUserRetrieveRequest.Password),
                new AlertTemplateDynamicContent (MEMBER_NAME , ObjUserRetrieveRequest.Name)};

                    AlertUtiltityParameters alertUtiltityParameters = new AlertUtiltityParameters
                   (UIConstants.EMAIL, ObjUserRetrieveRequest.EmailID, DynamicTemplate, ObjSmsEmailRetrieveResponseEmail.lstSmsEmailDetails[i]);

                    alertUtiltityParameters.ShouldUseThreading = false;
                    alertUtiltityParameters.MailHeaderText = UIConstants.OTP_ENROLLMENT_HEADER;
                    SendAlertUtility.SendAlert(alertUtiltityParameters);
                }
            }
            catch (Exception ex)
            {

                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + ":: SendOtpInEmail() " + ex.Message);
            }
        }

        private void SendDeactiveNotificationForCustomer(string Type, string CustomerName, string EmailMobile)
        {
            string Result = string.Empty;
            try
            {
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.CustomerAccountDeActivation);
                ObjSmsEmailRetrieveRequest.Type = Type;
                ObjSmsEmailRetrieveRequest.UserName = MERCHANT_USERNAME;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = EmailMobile;
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponseSMS = mobileAppBO.GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);
                if (ObjSmsEmailRetrieveResponseSMS.lstSmsEmailDetails.Count > 0)
                {

                    for (int i = 0; i < ObjSmsEmailRetrieveResponseSMS.lstSmsEmailDetails.Count; i++)
                    {

                        List<AlertTemplateDynamicContent> DynamicTemplate = new List<AlertTemplateDynamicContent>() {
                      new AlertTemplateDynamicContent("[Client name]", CustomerName)
                     };

                        if (Type == UIConstants.SMS)
                        {
                            AlertUtiltityParameters alertUtiltityParameters = new AlertUtiltityParameters
                                          (ObjSmsEmailRetrieveResponseSMS.lstSmsEmailDetails[i].Type, EmailMobile, DynamicTemplate, ObjSmsEmailRetrieveResponseSMS.lstSmsEmailDetails[i]);
                            alertUtiltityParameters.IsGetRequest = true;
                            if (DOMAIN == UIConstants.REDINGTON)
                                alertUtiltityParameters.VendorMethodName = SMSVendorMethodEnum.SmsCountry;
                            Result = SendAlertUtility.SendAlert(alertUtiltityParameters);
                        }
                        else if (Type == UIConstants.EMAIL)
                        {
                            AlertUtiltityParameters alertUtiltityParameters = new AlertUtiltityParameters
                                          (UIConstants.EMAIL, EmailMobile, DynamicTemplate, ObjSmsEmailRetrieveResponseSMS.lstSmsEmailDetails[i]);

                            alertUtiltityParameters.ShouldUseThreading = false;
                            SendAlertUtility.SendAlert(alertUtiltityParameters);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " : SendDeactiveNotificationForCustomer " + ex.Message);
            }
        }

        private void SendCustChildRegCredentials(string custUserName, string CustomerName, string Password, string Mobile, string LoyaltyId)
        {
            #region sms
            string Result = string.Empty;
            try
            {
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.CustomerTypesLoginCredentials);
                ObjSmsEmailRetrieveRequest.Type = UIConstants.SMS;
                ObjSmsEmailRetrieveRequest.UserName = MERCHANT_USERNAME;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = Mobile;
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponseSMS = mobileAppBO.GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);
                if (ObjSmsEmailRetrieveResponseSMS.lstSmsEmailDetails.Count > 0)
                {

                    for (int i = 0; i < ObjSmsEmailRetrieveResponseSMS.lstSmsEmailDetails.Count; i++)
                    {

                        List<AlertTemplateDynamicContent> DynamicTemplate = new List<AlertTemplateDynamicContent>() {
                      new AlertTemplateDynamicContent("[Client name]", CustomerName),
                      new AlertTemplateDynamicContent("[membership id]", LoyaltyId),
                      new AlertTemplateDynamicContent("[Pin]", Password)
                     };

                        AlertUtiltityParameters alertUtiltityParameters = new AlertUtiltityParameters
                                      (ObjSmsEmailRetrieveResponseSMS.lstSmsEmailDetails[i].Type, Mobile, DynamicTemplate, ObjSmsEmailRetrieveResponseSMS.lstSmsEmailDetails[i]);
                        alertUtiltityParameters.IsGetRequest = true;
                        if (DOMAIN == UIConstants.REDINGTON)
                            alertUtiltityParameters.VendorMethodName = SMSVendorMethodEnum.SmsCountry;
                        Result = SendAlertUtility.SendAlert(alertUtiltityParameters);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " : SendCustChildRegCredentials " + ex.Message);
            }
            #endregion sms

        }

        //For customer registration sms
        private void SendCustChildRegCredentialsEmail(string custUserName, string CustomerName, string Password, string Email, string LoyaltyId)
        {
            #region Email
            string Result = string.Empty;
            try
            {
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.CustomerTypesLoginCredentials);
                ObjSmsEmailRetrieveRequest.Type = UIConstants.EMAIL;
                ObjSmsEmailRetrieveRequest.UserName = MERCHANT_USERNAME;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = Email;
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponseSMS = mobileAppBO.GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);
                if (ObjSmsEmailRetrieveResponseSMS.lstSmsEmailDetails.Count > 0)
                {
                    for (int i = 0; i < ObjSmsEmailRetrieveResponseSMS.lstSmsEmailDetails.Count; i++)
                    {

                        List<AlertTemplateDynamicContent> DynamicTemplate = new List<AlertTemplateDynamicContent>() {
                      new AlertTemplateDynamicContent("[Client name]", CustomerName),
                      new AlertTemplateDynamicContent("[membership id]", LoyaltyId),
                      new AlertTemplateDynamicContent("[Pin]", Password)
                     };

                        AlertUtiltityParameters alertUtiltityParameters = new AlertUtiltityParameters
                                      (UIConstants.EMAIL, Email, DynamicTemplate, ObjSmsEmailRetrieveResponseSMS.lstSmsEmailDetails[i]);
                        alertUtiltityParameters.IsGetRequest = true;
                        alertUtiltityParameters.MailHeaderText = UIConstants.WELCOME_HEADER;
                        Result = SendAlertUtility.SendAlert(alertUtiltityParameters);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " : SendCustChildRegCredentialsEmail " + ex.Message);
            }
            #endregion sms
        }

        #endregion SMS & Email

        #region For Customer existancy & Member Verification
        public int CheckCustomerExistancyAndVerification(LocationSaveRequest ObjLocationSaveRequest)
        {
            LocationSaveResponse ObjLocationSaveResponse = null;
            int ReturnValue = 0;
            try
            {
                ObjLocationSaveResponse = mobileAppBO.CheckEmailMobileExistsMobileApp(ObjLocationSaveRequest);
                ReturnValue = ObjLocationSaveResponse.ReturnValue;
            }
            catch (Exception ex)
            {

                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " CheckCustomerExistancyAndVerifications() ::" + ex.Message);
            }
            return ReturnValue;
        }
        #endregion For Customer existancy & Member Verification

        #region Divya Nair

        public CatalogueRetriveResponseMobileApiJson GetCatalogueDetails(CatalogueRetriveRequestMobileApiJson ObjCatalogueRetriveRequest)
        {
            CatalogueRetriveResponseMobileApiJson objCatalogueRetriveResponse = null;
            try
            {
                ObjCatalogueRetriveRequest.Domain = DOMAIN;
                objCatalogueRetriveResponse = mobileAppBO.GetCatalogueDetails(ObjCatalogueRetriveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetCatalogueDetails() : " + ex.Message);
            }

            return objCatalogueRetriveResponse;
        }

        public CatalogueRetriveResponseJson GetCatalogueCategoryDetails(CatalogueRetriveRequestJson ObjCatalogueRetriveRequest, string ToTranslateLanguage)
        {
            CatalogueRetriveResponseJson objCatalogueRetriveResponse = null;
            try
            {

                if (ObjCatalogueRetriveRequest.ActionType == (int)Enumeration_Loyalty.ActionType.ActiveListForDDL)
                {
                    objCatalogueRetriveResponse = mobileAppBO.GetCatalogueCategoryDetails(ObjCatalogueRetriveRequest);
                    objCatalogueRetriveResponse.ObjCatalogueCategoryListJson = objCatalogueRetriveResponse.ObjCatalogueCategoryListJson.GroupBy(x => new { x.CatogoryId }).Select(group => group.First()).ToList();

                    if (!string.IsNullOrEmpty(ToTranslateLanguage))
                    {
                        List<Translation> results = google.Translate(UIConstants.DefalutLanguage, ToTranslateLanguage, (objCatalogueRetriveResponse.ObjCatalogueCategoryListJson.Select(x => x.CatogoryName).ToArray()));
                        for (int i = 0; i < objCatalogueRetriveResponse.ObjCatalogueCategoryListJson.Count; i++)
                        {
                            objCatalogueRetriveResponse.ObjCatalogueCategoryListJson[i].CatogoryName = results[i].TranslatedText;
                        }
                    }
                }
                else if (ObjCatalogueRetriveRequest.ActionType == (int)Enumeration_Loyalty.ActionType.ListAllSubCategory)
                {
                    objCatalogueRetriveResponse = mobileAppBO.GetCatalogueCategoryDetails(ObjCatalogueRetriveRequest);
                    objCatalogueRetriveResponse.ObjCatalogueCategoryListJson = objCatalogueRetriveResponse.ObjCatalogueCategoryListJson.GroupBy(x => new { x.SubCategoryID }).Select(group => group.First()).ToList();
                }
                else
                    objCatalogueRetriveResponse = mobileAppBO.GetCatalogueCategoryDetails(ObjCatalogueRetriveRequest);

            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "GetCatalogueCategoryDetails : " + ex.Message);
            }
            return objCatalogueRetriveResponse;
        }

        #endregion

        public LoyaltyProgRetrieveResponse GetLoyaltyProgramsForERequest(LoyaltyProgRetrieveRequest objLoyaltyProgRetrieveRequest)
        {
            LoyaltyProgRetrieveResponse objLoyaltyProgRetrieveResponse = null;
            try
            {
                objLoyaltyProgRetrieveResponse = mobileAppBO.GetLoyaltyProgramsForERequest(objLoyaltyProgRetrieveRequest);
            }
            catch (Exception ex)
            {

                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " : GetLoyaltyProgramsForERequest : " + ex.Message);
            }
            return objLoyaltyProgRetrieveResponse;
        }

        public BillingSaveResponse SaveEGiftRequest(BillingSaveRequest ObjBillingSaveRequest)
        {
            BillingSaveResponse objBillingSaveResponse = null;
            try
            {
                string FullPath = string.Empty, FileName = string.Empty;
                FullPath = HTTP_UPLOAD_FOLDER_PATH + PROFILE_PIC_FOLDER_PATH;
                //Display Image
                if (!string.IsNullOrEmpty(ObjBillingSaveRequest.objBilling.AuthenticityCardImgPath) && ObjBillingSaveRequest.objBilling.IsNewImg)
                {
                    FileName = "AuthenticityCard_" + ObjBillingSaveRequest.objBilling.LoyaltyId + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".png";
                    System.Drawing.Image CustomerImage = UICommon.Base64ToImageConverter(ObjBillingSaveRequest.objBilling.AuthenticityCardImgPath);

                    CustomerImage.Save((FullPath.Replace("~", "") + FileName), ImageFormat.Png);
                    ObjBillingSaveRequest.objBilling.AuthenticityCardImgPath = PROFILE_PIC_FOLDER_PATH.Replace("\\", "/") + FileName;
                }

                objBillingSaveResponse = mobileAppBO.SaveEGiftRequest(ObjBillingSaveRequest);

                if (objBillingSaveResponse != null)
                {
                    string[] arg = objBillingSaveResponse.ReturnMessage.Split('~');
                    string ReturnValue = arg[0];
                    if (ReturnValue == UIConstants.VALUE_THREE && arg.Count() == 3)
                    {
                        SendRejectEmailToCustomer(arg[2], ObjBillingSaveRequest);
                    }
                }

            }
            catch (Exception ex)
            {

                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " : SaveEGiftRequest() : " + ex.Message);
            }

            return objBillingSaveResponse;
        }

        public LoyaltySummaryResponseJson GetPendingAndRejectedEgiftRequests(LoyaltySummaryRequest objLoyaltySummaryRequest)
        {
            LoyaltySummaryResponseJson objLoyaltySummaryResponse = null;
            try
            {
                objLoyaltySummaryResponse = mobileAppBO.GetPendingAndRejectedEgiftRequests(objLoyaltySummaryRequest);
            }
            catch (Exception ex)
            {

                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " : GetPendingAndRejectedEgiftRequests() : " + ex.Message);
            }

            return objLoyaltySummaryResponse;
        }
        public CustomerActiveResponse UserActiveOrInActive(CustomerActiveRequest ObjCustomerRetrieveRequest)
        {
            CustomerActiveResponse objCustomerActiveResponse = null;
            try
            {
                objCustomerActiveResponse = mobileAppBO.UserActiveOrInActive(ObjCustomerRetrieveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "UserActiveOrInActive()" + ex.Message);
            }
            return objCustomerActiveResponse;
        }

        public CatalogueSaveResponse SaveCatalogueRedemptionDetailsEssilor(CatalogueSaveRequest ObjCatalogueSaveRequest)
        {
            CatalogueSaveResponse objCatalogueSaveResponse = null;
            Common.Contract.DataContracts.api.APIcredentialsresponse objAPIcredentialsResponse = null;
            string dataString = string.Empty, Referenceid = string.Empty, Orderid = string.Empty, Status = string.Empty,
                barcode = string.Empty, pdf_link = string.Empty, expiry_date = string.Empty, error_code = string.Empty,
                OrderResponse = string.Empty, APIKey = string.Empty, APISecreteKey = string.Empty, VoucherCardNumber = string.Empty;
            try
            {
                Uri requestUri;
                objCatalogueSaveResponse = mobileAppBO.SaveCatalogueRedemptionDetails(ObjCatalogueSaveRequest);

                MerchantConfigurationRetriveRequest objMerchantConfigurationRetriveRequest = new MerchantConfigurationRetriveRequest();
                objMerchantConfigurationRetriveRequest.MerchantID = ObjCatalogueSaveRequest.MerchantId;
                MerchantConfigurationRetriveResponse objMerchantConfigurationRetriveResponse = mobileAppBO.BindMerchatConfigureDetails(objMerchantConfigurationRetriveRequest);
                if (objMerchantConfigurationRetriveResponse != null && objMerchantConfigurationRetriveResponse.lstMerchantConfigRetriveList != null && objMerchantConfigurationRetriveResponse.lstMerchantConfigRetriveList.Count > 0)
                {
                    if (!string.IsNullOrEmpty(VoucherVendor))
                    {
                        List<MerchantConfiguration> lstMerchantConfigurationIdentity = objMerchantConfigurationRetriveResponse.lstMerchantConfigRetriveList.Where(a => a.ModuleName == "E_VOUCHER_DELIVERY" && a.ControlName == "AUTO").ToList();
                        if (lstMerchantConfigurationIdentity[0].Is_Mandatory)
                        {
                            // Get API credentials from DB Based on Vendor
                            objAPIcredentialsResponse = mobileAppBO.GetAPIcredentialsDetails(ObjCatalogueSaveRequest.ObjCatalogueList[0].CatalogueId, 0, 0);

                            APIKey = objAPIcredentialsResponse.APIcredentials.APIKey;
                            APISecreteKey = objAPIcredentialsResponse.APIcredentials.APISecreteKey;

                            if (GetWalletBalance(ObjCatalogueSaveRequest.ActorId, ObjCatalogueSaveRequest.MerchantId, APIKey, APISecreteKey) > 0)
                            {
                                if ((objCatalogueSaveResponse != null) && (!string.IsNullOrEmpty(objCatalogueSaveResponse.ReturnMessage)) && (objCatalogueSaveResponse.ReturnMessage != "RR-0-1") && (objCatalogueSaveResponse.ReturnMessage != "RR-00-1")) // ALSO BALANCE
                                {

                                    if (objCatalogueSaveResponse != null)
                                    {

                                        foreach (Catalogue item in ObjCatalogueSaveRequest.ObjCatalogueList)
                                        {
                                            if (ObjCatalogueSaveRequest.Vendor == UIConstants.YOU_GOT_A_GIFT)
                                            {
                                                Common.Contract.DataContracts.api.YouGotaGiftOrder ObjYouGotaGiftOrder = new Common.Contract.DataContracts.api.YouGotaGiftOrder();
                                                ObjYouGotaGiftOrder.reference_id = objCatalogueSaveResponse.ReturnMessage;
                                                ObjYouGotaGiftOrder.brand_code = item.ProductCode;
                                                ObjYouGotaGiftOrder.currency = item.CountryCurrencyCode;
                                                ObjYouGotaGiftOrder.amount = item.PointsRequired;
                                                ObjYouGotaGiftOrder.country = ObjCatalogueSaveRequest.CountryCode;
                                                ObjYouGotaGiftOrder.receiver_name = ObjCatalogueSaveRequest.ReceiverName;

                                                if (DOMAIN == UIConstants.CCRLP || DOMAIN == UIConstants.MICROSOFT_SURFACE_SELLER)
                                                {
                                                    ObjYouGotaGiftOrder.notify = 0;
                                                    ObjYouGotaGiftOrder.message = "Dear " + ObjCatalogueSaveRequest.ReceiverName + ", Thank you for redeeming. ";
                                                    ObjYouGotaGiftOrder.receiver_email = "";
                                                }
                                                else
                                                {
                                                    ObjYouGotaGiftOrder.delivery_type = 0;
                                                    ObjYouGotaGiftOrder.receiver_email = ObjCatalogueSaveRequest.ReceiverEmail;
                                                    ObjYouGotaGiftOrder.message = "Dear " + ObjCatalogueSaveRequest.ReceiverName + ", Thank you for redeeming. Kindly present this E-Gift card at the time of billing to claim your My Essilor Incentive gift amount.";
                                                }
                                                // Date in UTC
                                                string utcdate = DateTime.UtcNow.ToString("s") + "Z";
                                                string stringtosign = "x-date: " + utcdate;
                                                string sign = CreateSignature(APISecreteKey, stringtosign);
                                                string authorizationHeader = "Signature headers=\"x-date\",keyId=\"" + APIKey + "\",algorithm=\"hmac-sha256\",signature=\"" + sign + "\"";

                                                StringBuilder sb = new StringBuilder();
                                                sb.AppendLine("Authorization:" + authorizationHeader + " ");
                                                headers = sb.ToString();
                                                requestUri = new Uri(APIURL);
                                                ResponseString = WebRequestPostMethod(requestUri, headers, ObjYouGotaGiftOrder, utcdate, APIKey);

                                                if (ResponseString != "401")
                                                {
                                                    JObject joResponse = JObject.Parse(ResponseString);
                                                    Referenceid = Convert.ToString(joResponse["reference_id"]);
                                                    Orderid = Convert.ToString(joResponse["order_id"]);
                                                    Status = Convert.ToString(joResponse["state"]);

                                                    if (DOMAIN == UIConstants.CCRLP || DOMAIN == UIConstants.MICROSOFT_SURFACE_SELLER)
                                                    {
                                                        barcode = Convert.ToString(joResponse["barcode"]);
                                                        pdf_link = Convert.ToString(joResponse["pdf_link"]);
                                                        expiry_date = Convert.ToString(joResponse["expiry_date"]);
                                                    }
                                                }
                                                else
                                                {
                                                    ObjCatalogueSaveRequest.ResponseStatus = Convert.ToString(ResponseString);
                                                    ObjCatalogueSaveRequest.ActionType = 2;
                                                    ObjCatalogueSaveRequest.Redemption_ref_no = objCatalogueSaveResponse.ReturnMessage;
                                                    objCatalogueSaveResponse.ReturnMessage = "RR-000-1";
                                                }
                                            }
                                            else if (ObjCatalogueSaveRequest.Vendor == UIConstants.SURE_GIFT)
                                            {
                                                Common.Utility.SureGiftRequest RedemptionRequest = new Common.Utility.SureGiftRequest();
                                                RedemptionRequest.DeliveryDate = DateTime.Now.ToString();
                                                RedemptionRequest.OrderItemRequests = new List<Common.Utility.OrderItemRequest>();
                                                Common.Utility.OrderItemRequest ObjOrderItemRequest = new Common.Utility.OrderItemRequest();

                                                ObjOrderItemRequest.MerchantId = item.ProductCode;
                                                ObjOrderItemRequest.EventName = "My Essilor Rewards | E-Gift card";
                                                ObjOrderItemRequest.Amount = item.PointsRequired;
                                                ObjOrderItemRequest.Quantity = 1;
                                                ObjOrderItemRequest.RecipientEmail = ObjCatalogueSaveRequest.ReceiverEmail;
                                                ObjOrderItemRequest.RecipientFirstName = ObjCatalogueSaveRequest.ReceiverName;
                                                ObjOrderItemRequest.RecipientLastName = "   ";
                                                ObjOrderItemRequest.RecipientPhone = ObjCatalogueSaveRequest.ReceiverMobile;
                                                string SureGiftURL = string.Empty;
                                                if (ObjCatalogueSaveRequest.MerchantId == 1)
                                                {
                                                    SureGiftURL = SURE_GIFT_ORDER_API_K;
                                                    ObjOrderItemRequest.TemplateId = SURE_GIFT_TEMPLATE_ID_K;
                                                }
                                                else
                                                {
                                                    SureGiftURL = SURE_GIFT_ORDER_API_N;
                                                    ObjOrderItemRequest.TemplateId = SURE_GIFT_TEMPLATE_ID_N;
                                                }
                                                RedemptionRequest.OrderItemRequests.Add(ObjOrderItemRequest);


                                                string Json = JsonConvert.SerializeObject(RedemptionRequest);

                                                ErrorHandler.WriteErrorToPhysicalPath("Request: " + Json);
                                                Skipper.Common.Utility.HttpRequestUtility.HttpRequestArguments httpRequestArguments = new Skipper.Common.Utility.HttpRequestUtility.HttpRequestArguments(SureGiftURL, Json, "application/json", false, "");
                                                httpRequestArguments.Headers = new WebHeaderCollection();
                                                httpRequestArguments.Headers.Add("Authorization", APIKey);
                                                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                                                string ResponseString = Skipper.Common.Utility.HttpRequestUtility.HttpRequest.MakePostRequest(httpRequestArguments);
                                                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " SaveCatalogueRedemptionDetailsEssilor() : " + ResponseString);
                                                if (!string.IsNullOrEmpty(ResponseString))
                                                {
                                                    JObject joResponse = JObject.Parse(ResponseString);

                                                    JArray Voucher_Array = (JArray)joResponse["Data"];

                                                    foreach (JObject Items_voucher in Voucher_Array)
                                                    {
                                                        Orderid = Convert.ToString(Items_voucher["OrderId"]);
                                                        expiry_date = Convert.ToString(Items_voucher["VoucherExpiryDate"]);
                                                        JArray Voucher = (JArray)(Items_voucher["Voucher"]);
                                                        VoucherCardNumber = Voucher[0].ToString();
                                                        Status = "COMPLETE";
                                                    }
                                                }
                                            }

                                            ObjCatalogueSaveRequest.ResponseReferenceId = Referenceid;
                                            ObjCatalogueSaveRequest.ResponseOderId = Orderid;
                                            ObjCatalogueSaveRequest.ResponseStatus = Status;
                                            ObjCatalogueSaveRequest.Redemption_ref_no = objCatalogueSaveResponse.ReturnMessage;
                                            ObjCatalogueSaveRequest.ReceiverEmail = ObjCatalogueSaveRequest.ReceiverEmail;
                                            ObjCatalogueSaveRequest.Barcode = barcode;
                                            ObjCatalogueSaveRequest.pdf_link = pdf_link;
                                            ObjCatalogueSaveRequest.expiry_date = expiry_date;
                                            ObjCatalogueSaveRequest.VoucherAmount = item.PointsRequired;
                                            ObjCatalogueSaveRequest.VoucherCardNumber = UICommon.Encrypt(VoucherCardNumber);
                                            if (Status == "PROCESSING")
                                            {
                                                ObjCatalogueSaveRequest.ResponseStatus = Convert.ToString(error_code);
                                                ObjCatalogueSaveRequest.ActionType = 2;
                                                ObjCatalogueSaveRequest.Redemption_ref_no = objCatalogueSaveResponse.ReturnMessage;
                                                objCatalogueSaveResponse.ReturnMessage = "RR-000-1";
                                            }


                                            CatalogueSaveResponse ObjCatalogueSaveResponseORDER = mobileAppBO.SaveCatalogueOrderDetails(ObjCatalogueSaveRequest);
                                            if (ObjCatalogueSaveResponseORDER != null)
                                            {
                                                int i = ObjCatalogueSaveResponseORDER.ReturnValue;

                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (DOMAIN == UIConstants.GVLC)
                {
                    if (objCatalogueSaveResponse.ReturnMessage != "RR-0-1" && objCatalogueSaveResponse.ReturnMessage != "RR-00-1" && objCatalogueSaveResponse.ReturnValue >= 0)
                    {

                        CustomerRetrieveResponse ObjCustomerRetrieveResponse = null;
                        CustomerRetrieveRequest objCustomerRetrieveRequest = new CustomerRetrieveRequest();
                        objCustomerRetrieveRequest.ActionType = Convert.ToInt32(UIConstants.VALUE_THREE);
                        objCustomerRetrieveRequest.ActorId = objCatalogueSaveResponse.UserId;
                        ObjCustomerRetrieveResponse = mobileAppBO.GetUserDetailsForPushNotification(objCustomerRetrieveRequest);

                        if (ObjCustomerRetrieveResponse != null && ObjCustomerRetrieveResponse.lstCustomer != null && ObjCustomerRetrieveResponse.lstCustomer.Count >= 0)
                        {
                            foreach (var name in ObjCustomerRetrieveResponse.lstCustomer)
                            {
                                SendPushNotificationsToUser(name.UserName, name.Mobile, name.FirstName, name.CustomerType);
                            }
                        }
                    }

                }

            }
            catch (WebException we) // TO SAVE status Code
            {
                int StatusCode;
                StatusCode = (int)((HttpWebResponse)we.Response).StatusCode;
                ErrorHandler.WriteErrorToPhysicalPath("StatusCode: " + StatusCode);
                ObjCatalogueSaveRequest.ResponseStatus = Convert.ToString(StatusCode);
                ObjCatalogueSaveRequest.ActionType = 2;
                objCatalogueSaveResponse.ReturnMessage = "RR-000-1";
                ObjCatalogueSaveRequest.Redemption_ref_no = objCatalogueSaveResponse.ReturnMessage;
                CatalogueSaveResponse ObjCatalogueSaveResponseORDER = mobileAppBO.SaveCatalogueOrderDetails(ObjCatalogueSaveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " SaveCatalogueRedemptionDetailsEssilor() : " + ex.ToString());
                if (!string.IsNullOrEmpty(objCatalogueSaveResponse.ReturnMessage))
                {
                    ObjCatalogueSaveRequest.ResponseStatus = "500"; //internal Error
                    ObjCatalogueSaveRequest.ActionType = 2;
                    ObjCatalogueSaveRequest.Redemption_ref_no = objCatalogueSaveResponse.ReturnMessage;
                    CatalogueSaveResponse ObjCatalogueSaveResponseORDER = mobileAppBO.SaveCatalogueOrderDetails(ObjCatalogueSaveRequest);
                }
                objCatalogueSaveResponse.ReturnMessage = "RR-000-1";
            }
            return objCatalogueSaveResponse;
        }


        private void SendPushNotificationsToUser(string UserName, string Mobile, string FirstName, string CustomerType)
        {
            try
            {
                PushHistoryRequest ObjPushHistoryRequest = new PushHistoryRequest();
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse = new SmsEmailRetrieveResponse();
                List<string> deviceListString = new List<string>();
                SmsEmail ObjSmsEmail = new SmsEmail();
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();

                ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.CustomerRedemptionApprovalForAsm);

                ObjSmsEmailRetrieveRequest.UserName = UserName;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = Mobile;
                ObjSmsEmailRetrieveRequest.Type = "SMS";
                ObjSmsEmailRetrieveRequest.UserType = CustomerType;




                ObjSmsEmailRetrieveResponse = mobileAppBO.GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);
                ObjPushHistoryRequest.LoyaltyId = ObjSmsEmailRetrieveRequest.UserName;
                ObjPushHistoryRequest.MessageId = ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[0].Template_Id;
                ObjPushHistoryRequest.Message = ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[0].Template;
                ObjPushHistoryRequest.LPushType = (int)Enumeration_Loyalty.LPushType.Catalogue;
                for (int k = 0; k <= ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count - 1; k++)
                {
                    List<AlertTemplateDynamicContent> DynamicTemplateEmail = new List<AlertTemplateDynamicContent>()
                        {
                             new AlertTemplateDynamicContent("[MEMBER]", FirstName),
                             new AlertTemplateDynamicContent("[MEMBER TYPE]", CustomerType),
                        };

                    AlertUtiltityParameters alertUtiltityParameterEmail = new AlertUtiltityParameters(ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[k].Type, Mobile, DynamicTemplateEmail, ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[k]);

                    var MailResult = SendAlertUtility.SendAlert(alertUtiltityParameterEmail);
                }
                SavePushHistoryDetails(ObjPushHistoryRequest);

            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "SendPushNotificationsToUser() : " + ex.Message);
            }
        }

        // Creating Signature
        private static string CreateSignature(string secrete, string stringtosign)
        {
            string signature = string.Empty;
            try
            {
                var secretBytes = Encoding.UTF8.GetBytes(secrete);
                var valueBytes = Encoding.UTF8.GetBytes(stringtosign);


                using (var hmac = new HMACSHA256(secretBytes))
                {
                    var hash = hmac.ComputeHash(valueBytes);
                    signature = Convert.ToBase64String(hash);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " CreateSignature() : " + ex.Message);
            }
            return signature;
        }

        public CustomerSaveResponse UpdateCustomerProfileMobileApp(CustomerSaveRequest ObjCustomerSaveRequest)
        {
            CustomerSaveResponse objCustomerSaveResponse = null;
            try
            {
                string FullPath = string.Empty, FileName = string.Empty;
                FullPath = HTTP_UPLOAD_FOLDER_PATH + PROFILE_PIC_FOLDER_PATH;
                if (!string.IsNullOrEmpty(ObjCustomerSaveRequest.ObjCustomer.DisplayImage))
                {
                    FileName = ObjCustomerSaveRequest.ObjCustomer.LoyaltyId + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".png";
                    //System.Drawing.Image CustomerImage = UICommon.Base64ToImageConverter(ObjCustomerSaveRequest.ObjCustomer.DisplayImage);

                    //CustomerImage.Save((FullPath.Replace("~", "") + FileName), ImageFormat.Png);
                    UICommon.SaveBase64StringIntoFile(FullPath.Replace("~", ""), ObjCustomerSaveRequest.ObjCustomer.DisplayImage, FileName);
                    ObjCustomerSaveRequest.ObjCustomer.DisplayImage = FileName;
                    ObjCustomerSaveRequest.ObjCustomer.Domain = DOMAIN;
                    ObjCustomerSaveRequest.ActionType = Convert.ToInt16(Enumeration_Loyalty.ActionType.UpdateCustImagePath);
                    objCustomerSaveResponse = mobileAppBO.SaveCustomerRegistrationDetailsMobileApp(ObjCustomerSaveRequest);

                    if (ObjCustomerSaveRequest.ActionType == Convert.ToInt16(Enumeration_Loyalty.ActionType.UpdateCustImagePath))
                    {
                        if (objCustomerSaveResponse.ReturnMessage.Contains("~"))
                        {
                            if (objCustomerSaveResponse.ReturnMessage.Split('~')[1].ToString() == UIConstants.VALUE_ONE)
                            {
                                objCustomerSaveResponse.ReturnMessage = UIConstants.VALUE_ONE + PROFILE_PIC_FOLDER_PATH.Replace("\\", "/") + FileName;
                            }
                            else
                            {
                                objCustomerSaveResponse.ReturnMessage = UIConstants.VALUE_MINUS_ONE;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " UpdateCustomerProfileMobileApp() : " + ex.Message);
            }

            return objCustomerSaveResponse;
        }

        public string UpdateCustomerPushID(int ActorId, string PushId)
        {
            string objResponse = null;
            try
            {
                objResponse = mobileAppBO.UpdateCustomerPushID(ActorId, PushId);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " UpdateCustomerPushID() : " + ex.Message);
            }

            return objResponse;
        }

        public BillingSaveResponse SavePurchaseRequest(ProductSaveRequest objProdSaveRequest)
        {
            BillingSaveResponse BillingSaveResponse = null;
            try
            {
                BillingSaveResponse = mobileAppBO.SavePurchaseRequest(objProdSaveRequest);
                if (BillingSaveResponse != null)
                {
                    if (BillingSaveResponse.ReturnMessage == "1")
                    {
                        SendPointsAccumulationSms(MERCHANT_USERNAME, BillingSaveResponse.FirstName, BillingSaveResponse.Mobile
                                                    , Convert.ToString(BillingSaveResponse.CreditedPoints), Convert.ToString(BillingSaveResponse.PointsBalance));
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "SavePurchaseRequest : " + ex.Message);
            }
            return BillingSaveResponse;
        }

        public CustomerRetrieveResponse GetCustomerDetails(CustomerRetrieveRequest ObjCustomerRetrieveRequest)
        {
            CustomerRetrieveResponse objCustomerRetrieveResponse = null;
            try
            {

                objCustomerRetrieveResponse = mobileAppBO.GetCustomerDetails(ObjCustomerRetrieveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "GetCustomerDetails : " + ex.Message);
            }
            return objCustomerRetrieveResponse;
        }

        public LocationRetrieveResponse GetLocationBasedOnArea(AreaRetrieveRequest objAreaRetrieveRequest)
        {
            LocationRetrieveResponse objLocationRetrieveResponse = null;
            try
            {
                objLocationRetrieveResponse = mobileAppBO.GetLocationBasedOnArea(objAreaRetrieveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "GetLocationBasedOnArea : " + ex.Message);
            }
            return objLocationRetrieveResponse;
        }

        public string UpdateCustomerLocationID(int ActorId, string LocationID)
        {
            string objResponse = null;
            try
            {
                objResponse = mobileAppBO.UpdateCustomerLocationID(ActorId, LocationID);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " UpdateCustomerLocationID() : " + ex.Message);
            }

            return objResponse;
        }

        public BillingSaveResponse SaveBillingBudwiser(ProductSaveRequest objProdSaveRequest)
        {
            BillingSaveResponse BillingSaveResponse = null;
            try
            {
                string FullPath = string.Empty, FileName = string.Empty;
                FullPath = HTTP_UPLOAD_FOLDER_PATH + PROFILE_PIC_FOLDER_PATH;
                //Display Image
                if (objProdSaveRequest.CardImgPathGalleryList != null)
                {
                    if (objProdSaveRequest.CardImgPathGalleryList.Count > 0)
                    {
                        int rowindex = 0;
                        foreach (ProductSaveCardImgPathGallery item in objProdSaveRequest.CardImgPathGalleryList.ToList())
                        {

                            FileName = "Issues_" + objProdSaveRequest.ProductSaveDetailList[0].LoyalityID + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + "_" + rowindex + ".png";
                            Image CustomerImage = UICommon.Base64ToImageConverter(item.AuthenticityCardImgPath);

                            CustomerImage.Save((FullPath.Replace("~", "") + FileName), ImageFormat.Png);
                            item.AuthenticityCardImgPath = PROFILE_PIC_FOLDER_PATH.Replace("\\", "/") + FileName;

                            rowindex++;
                        }

                    }
                }
                BillingSaveResponse = mobileAppBO.SaveBillingBudwiser(objProdSaveRequest);
            }
            catch (Exception ex)
            {

                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " SaveBillingBudwiser() : " + ex.Message);
            }
            return BillingSaveResponse;
        }

        public UserLoginRetrieveResponse GetcustomerDetailsByLoyaltyId(UserRetrieveRequest_Loyalty userRetrieveRequest)
        {
            UserLoginRetrieveResponse objUserLoginRetrieveResponse = null;
            try
            {
                objUserLoginRetrieveResponse = mobileAppBO.CheckIsAuthenticatedMobileApp(userRetrieveRequest);
                objUserLoginRetrieveResponse.UserList[0].Result = 1;
                userRetrieveRequest.ActionType = SAVE_LOG_DETAILS;
                mobileAppBO.CheckIsAuthenticatedMobileApp(userRetrieveRequest);
                IssueAutomaticGiftcard(userRetrieveRequest.UserName, LOGIN, Convert.ToInt32(UIConstants.VALUE_TWO));
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "GetcustomerDetailsByLoyaltyId : " + ex.Message);
            }
            return objUserLoginRetrieveResponse;
        }

        public GetPointsExpiryResponse GetCustomerPointExpiryDetails(GetPointsExpiryRequest ObjGetPointsExpiryRequest)
        {
            GetPointsExpiryResponse objGetPointsExpiryResponse = null;
            try
            {
                objGetPointsExpiryResponse = mobileAppBO.GetCustomerPointExpiryDetails(ObjGetPointsExpiryRequest);
            }
            catch (Exception ex)
            {

                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "GetCustomerPointExpiryDetails()" + ex.Message);
            }

            return objGetPointsExpiryResponse;
        }

        public string SaveCustomerDetailsFromOpenId(string Name, string EmailId, string LoyaltyId, string ProfileImage, int CustomerType, int SourceDevice, string DeviceId, string UserIp, SaveCustomerDetailsOpenIdRequest objSaveCustomerDetailsOpenIdRequest, int UserId, int RegStatus = 0)
        {
            string objResponse = null;
            try
            {

                string Password = UICommon.GetRandomNumber();
                Password = Security.EncryptPassword(Password);

                if (UserId > 0)
                {
                    if (objSaveCustomerDetailsOpenIdRequest != null)
                    {
                        if (!string.IsNullOrEmpty(objSaveCustomerDetailsOpenIdRequest.OfficialBankletter) && objSaveCustomerDetailsOpenIdRequest.IsBankNewImg == 1)
                        {
                            string IdProofFolderPath = HTTP_UPLOAD_FOLDER_PATH + IDENTITY_PROOF_PIC_FOLDER_PATH.Replace("~", "");
                            string imageName = EmailId + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".png";
                            bool Result = UICommon.SaveBase64StringIntoFile(IdProofFolderPath, objSaveCustomerDetailsOpenIdRequest.OfficialBankletter, imageName);
                            objSaveCustomerDetailsOpenIdRequest.OfficialBankletter = imageName;
                        }
                        if (!string.IsNullOrEmpty(objSaveCustomerDetailsOpenIdRequest.PanTan) && objSaveCustomerDetailsOpenIdRequest.IsOtherNewImg == 1)
                        {
                            string IdProofFolderPath = HTTP_UPLOAD_FOLDER_PATH + IDENTITY_PROOF_PIC_FOLDER_PATH.Replace("~", "");
                            string imageName = EmailId + "_1" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".png";
                            bool Result = UICommon.SaveBase64StringIntoFile(IdProofFolderPath, objSaveCustomerDetailsOpenIdRequest.PanTan, imageName);
                            objSaveCustomerDetailsOpenIdRequest.PanTan = imageName;
                        }
                    }
                }
                objResponse = mobileAppBO.SaveCustomerDetailsFromOpenId(Name, EmailId, Password, ProfileImage, MERCHANT_USERNAME, CustomerType, SourceDevice, DeviceId, UserIp, objSaveCustomerDetailsOpenIdRequest, UserId);

                if (objSaveCustomerDetailsOpenIdRequest != null)
                {
                    if (objSaveCustomerDetailsOpenIdRequest.AccountStatus == 1 && objResponse != null && !string.IsNullOrEmpty(objResponse) && (RegStatus == 1 || RegStatus == 2))
                    {
                        if (RegStatus == 1)
                        {
                            sendCustomerWelcomeEmail(Name, EmailId);
                        }
                        if (RegStatus == 1 || RegStatus == 2)
                        {
                            if (RegStatus == 1)
                            {

                                SendPendingAccountAdmin(Name, EmailId, Enumeration_Loyalty.SmsEmailTemplates.PendingAccountAdmin.ToString(), "Pending");
                                SendSuccessfullRegistrationUpdationAlerts(Name, EmailId, LoyaltyId, Convert.ToInt32(objSaveCustomerDetailsOpenIdRequest.CountryName.Trim()), Enumeration_Loyalty.SmsEmailTemplates.CustomerRegistrationEmailAlertGp.ToString(), "Pending");
                            }
                            else
                            {
                                SendPendingAccountAdmin(Name, EmailId, Enumeration_Loyalty.SmsEmailTemplates.ProfileUpdatedAccountAdmin.ToString(), "Profile Updated");
                                SendSuccessfullRegistrationUpdationAlerts(Name, EmailId, LoyaltyId, Convert.ToInt32(objSaveCustomerDetailsOpenIdRequest.CountryName.Trim()), Enumeration_Loyalty.SmsEmailTemplates.CustomerProfileUpdateEmailAlertGp.ToString(), "Profile Updated");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " SaveCustomerDetailsFromOpenId() : " + ex.Message);
            }

            return objResponse;
        }
        private void sendCustomerWelcomeEmail(string CustomerName, string Email)
        {
            try
            {
                SmsEmail ObjSmsEmail = new SmsEmail();
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.WelcomeAccountactivation);
                ObjSmsEmailRetrieveRequest.Type = UIConstants.EMAIL;
                ObjSmsEmailRetrieveRequest.UserName = MERCHANT_USERNAME;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = Email;
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse = mobileAppBO.GetEmailSmsTemplate(ObjSmsEmailRetrieveRequest);
                for (int i = 0; i < ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count; i++)
                {
                    List<AlertTemplateDynamicContent> DynamicTemplate = new List<AlertTemplateDynamicContent>(){
                new AlertTemplateDynamicContent ("[Client name]",CustomerName)};

                    AlertUtiltityParameters alertUtiltityParameters = new AlertUtiltityParameters
                   (UIConstants.EMAIL, Email, DynamicTemplate, ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i]);
                    alertUtiltityParameters.IsGetRequest = true;

                    var Result = SendAlertUtility.SendAlert(alertUtiltityParameters);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " SaveCustomerDetailsFromOpenId() : " + ex.Message);
            }
        }

        private static string ResendAPI(Uri requestUri, string headers, string RefNo)
        {
            string JsonResponse = string.Empty;
            try
            {

                HttpWebResponse respFinal = WebRequestGetMethod(requestUri, headers);

                using (Stream responseStream = respFinal.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    JsonResponse = reader.ReadToEnd().ToString();
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " ResendAPI() : " + ex.Message);
            }
            return JsonResponse;

        }

        private static HttpWebResponse WebRequestGetMethod(Uri Url, string headers)
        {
            HttpWebResponse response = null;
            try
            {
                HttpWebRequest tRequest = WebRequest.Create(Url) as HttpWebRequest;
                tRequest.ContentType = "application/json";
                tRequest.Accept = "*/*";
                tRequest.Headers.Add(headers);

                response = tRequest.GetResponse() as HttpWebResponse;
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " WebRequestGetMethod() " + ex.Message);
            }

            return response;
        }

        private static string WebRequestPostMethod(Uri Url, string headers, object objData, string utcdate, string APIKey)
        {
            string responseString = string.Empty;
            HttpWebResponse response = null;
            try
            {
                HttpWebRequest tRequest = WebRequest.Create(Url) as HttpWebRequest;
                tRequest.ContentType = "application/json";
                tRequest.Accept = "*/*";
                tRequest.Headers.Add(headers);
                tRequest.Method = "POST";
                tRequest.Timeout = 50000;
                if (DOMAIN != UIConstants.BUDWEISER && DOMAIN != UIConstants.GodrejLocks && DOMAIN != UIConstants.HSE && DOMAIN != UIConstants.SKLP)
                {
                    tRequest.Headers.Add("X-Api-Key:" + APIKey + "");
                    tRequest.Headers.Add("X-Date:" + utcdate + "");
                }
                string dataString = JsonConvert.SerializeObject(objData);
                // turn our request string into a byte stream
                byte[] postBytes = Encoding.ASCII.GetBytes(dataString);
                // this is important - make sure you specify type this way
                tRequest.ContentType = "application/json";
                tRequest.ContentLength = postBytes.Length;
                Stream requestStream = tRequest.GetRequestStream();
                // now send it
                requestStream.Write(postBytes, 0, postBytes.Length);
                requestStream.Close();

                // grab te response and print it out to the console along with the status code
                response = (HttpWebResponse)tRequest.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream());
                HttpStatusCode statusCode = response.StatusCode;
                if (statusCode == HttpStatusCode.Created || statusCode == HttpStatusCode.OK)
                {
                    responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                }
            }

            catch (WebException we)
            {
                if (DOMAIN == UIConstants.BUDWEISER || DOMAIN == UIConstants.GodrejLocks || DOMAIN == UIConstants.HSE || DOMAIN == UIConstants.SKLP)
                {
                    if (we.Status == WebExceptionStatus.Timeout)
                    {
                        responseString = Convert.ToString(we.Status);
                        ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " WebRequestPostMethod() " + responseString);
                    }
                    else
                    {
                        int StatusCode = (int)((HttpWebResponse)we.Response).StatusCode;
                        responseString = Convert.ToString(StatusCode);
                    }
                }

                using (var stream = we.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " WebRequestPostMethod():Response " + reader.ReadToEnd());
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " WebRequestPostMethod() " + ex.Message);
            }

            return responseString;
        }

        public SMSCampaignRetriveResponse GetSMSCampaignDetails(SMSCampaignRetriveRequest objSMSCampaignRetriveRequest)
        {
            SMSCampaignRetriveResponse objSMSCampaignRetriveResponse = null;
            try
            {
                objSMSCampaignRetriveResponse = mobileAppBO.GetSMSCampaignDetails(objSMSCampaignRetriveRequest);
            }
            catch (Exception ex)
            {

                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "GetSMSCampaignDetails()" + ex.Message);
            }

            return objSMSCampaignRetriveResponse;
        }

        public PushHistoryResponce GetPushHistoryDetails(PushHistoryRequest objPushHistoryRequest)
        {
            PushHistoryResponce objPushHistoryResponce = null;
            try
            {
                objPushHistoryResponce = mobileAppBO.GetPushHistoryDetails(objPushHistoryRequest);
            }
            catch (Exception ex)
            {

                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "GetPushHistoryDetails()" + ex.Message);
            }

            return objPushHistoryResponce;
        }


        public PushHistoryResponce SavePushHistoryDetails(PushHistoryRequest objSavePushHistoryDetails)
        {
            PushHistoryResponce objPushHistoryResponce = null;
            try
            {
                objPushHistoryResponce = mobileAppBO.SavePushHistoryDetails(objSavePushHistoryDetails);
            }
            catch (Exception ex)
            {

                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "SavePushHistoryDetails()" + ex.Message);
            }

            return objPushHistoryResponce;
        }
        public CustomerRetrieveResponse GetMostPointsEarnedCutomer(CustomerRetrieveRequest objCustomerRetrieveRequest)
        {
            CustomerRetrieveResponse objCustomerRetrieveResponse = null;
            try
            {
                objCustomerRetrieveResponse = mobileAppBO.GetMostPointsEarnedCutomer(objCustomerRetrieveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "GetMostPointsEarnedCutomer()" + ex.Message);
            }
            return objCustomerRetrieveResponse;
        }

        public GetSurveyQuestionAnswerResponse GetSurveyQuestionAnswer(GetSurveyQuestionAnswerRequest objGetSurveyQuestionAnswerRequest)
        {
            GetSurveyQuestionAnswerResponse objGetSurveyQuestionAnswerResponce = null;
            try
            {
                objGetSurveyQuestionAnswerResponce = mobileAppBO.GetSurveyQuestionAnswer(objGetSurveyQuestionAnswerRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "GetSurveyQuestionAnswer()" + ex.Message);
            }
            return objGetSurveyQuestionAnswerResponce;
        }

        public SaveSurveyQuestionAnswerResponse SaveSurveyQuestionAnswer(SaveSurveyQuestionAnswerRequest objSaveSurveyQuestionAnswerRequest)
        {
            SaveSurveyQuestionAnswerResponse objGetSurveyQuestionAnswerResponce = null;
            try
            {
                string FullPath = string.Empty, FileName = string.Empty;
                FullPath = HTTP_UPLOAD_FOLDER_PATH + PROFILE_PIC_FOLDER_PATH;
                foreach (SurveyQuestion Name in objSaveSurveyQuestionAnswerRequest.lstSurveyQuestion)
                {
                    if (!string.IsNullOrEmpty(Name.ImagePath))
                    {
                        FileName = "SurveyQuestion_" + objSaveSurveyQuestionAnswerRequest.ActorId + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + "_" + ".png";
                        Image CustomerImage = UICommon.Base64ToImageConverter(Name.ImagePath);
                        CustomerImage.Save((FullPath.Replace("~", "") + FileName), ImageFormat.Png);
                        Name.ImagePath = PROFILE_PIC_FOLDER_PATH.Replace("\\", "/") + FileName;
                    }
                }
                objSaveSurveyQuestionAnswerRequest.Domain = DOMAIN;
                objGetSurveyQuestionAnswerResponce = mobileAppBO.SaveSurveyQuestionAnswer(objSaveSurveyQuestionAnswerRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "SaveSurveyQuestionAnswer()" + ex.Message);
            }
            return objGetSurveyQuestionAnswerResponce;
        }
        public CatalogueRetriveResponse GetCatalogueDetailsInventory(CatalogueRetriveRequest ObjCatalogueRetriveRequest)
        {
            CatalogueRetriveResponse objCatalogueRetriveResponse = null;
            try
            {
                objCatalogueRetriveResponse = mobileAppBO.GetCatalogueDetailsInventory(ObjCatalogueRetriveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetCatalogueDetailsInventory() : " + ex.Message);
            }
            return objCatalogueRetriveResponse;
        }
        public CatalogueSaveResponse SaveCatalogueRedemptionInventoryDetails(CatalogueSaveRequest ObjCatalogueSaveRequest)
        {
            string[] pointsBalance;
            CatalogueSaveResponse objCatalogueSaveResponse = null;
            try
            {
                objCatalogueSaveResponse = mobileAppBO.SaveCatalogueRedemptionInventoryDetails(ObjCatalogueSaveRequest);
                if ((objCatalogueSaveResponse != null) && (!string.IsNullOrEmpty(objCatalogueSaveResponse.ReturnMessage)) && (objCatalogueSaveResponse.ReturnMessage != "RR-0-1") && (objCatalogueSaveResponse.ReturnMessage != "RR-00-1")) // ALSO BALANCE
                {
                    pointsBalance = objCatalogueSaveResponse.ReturnMessage.Split('~');
                    if (pointsBalance[0] != UIConstants.VALUE_MINUS_ONE)
                    {
                        SendSMSForSuccessfulRedemption(pointsBalance[1], ObjCatalogueSaveRequest);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " SaveCatalogueRedemptionInventoryDetails() : " + ex.Message);
            }
            return objCatalogueSaveResponse;
        }
        private void SendSMSForSuccessfulRedemption(string PointsBalance, CatalogueSaveRequest ObjCatalogueSaveRequest)
        {
            try
            {
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.PointDebitFromLocation_EXE);
                ObjSmsEmailRetrieveRequest.Type = UIConstants.SMS;
                ObjSmsEmailRetrieveRequest.UserName = ObjCatalogueSaveRequest.ObjCatalogueList[0].LoyaltyId;
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse = mobileAppBO.GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);
                for (int i = 0; i < ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count; i++)
                {
                    List<AlertTemplateDynamicContent> DynamicTemplate = new List<AlertTemplateDynamicContent>(){
                    new AlertTemplateDynamicContent ("[points]",Convert.ToString(ObjCatalogueSaveRequest.ObjCatalogueList[0].NoOfPointsDebit )),
                    new AlertTemplateDynamicContent ("[BALANCE]",Convert.ToString(PointsBalance)),
                    new AlertTemplateDynamicContent ("[Voucher_code]",Convert.ToString(ObjCatalogueSaveRequest.ObjCatalogueList[0].ProductCode ))
                    };

                    AlertUtiltityParameters alertUtiltityParameters = new AlertUtiltityParameters
                   (ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i].Type, ObjCatalogueSaveRequest.ReceiverMobile, DynamicTemplate, ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i]);
                    alertUtiltityParameters.IsGetRequest = true;

                    var Result = SendAlertUtility.SendAlert(alertUtiltityParameters);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "SendSMSForSuccessfulRedemption()" + ex.Message);
            }
        }
        private void SendRejectEmailToCustomer(string Remark, BillingSaveRequest ObjBillingSaveRequest)
        {
            try
            {
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.RejectedTransactionsRequest);
                ObjSmsEmailRetrieveRequest.UserName = ObjBillingSaveRequest.objBilling.LoyaltyId;

                ObjSmsEmailRetrieveRequest.EmailOrMobile = ObjBillingSaveRequest.objBilling.LoyaltyId;
                // Getting E-Mail Template
                ObjSmsEmailRetrieveRequest.Type = UIConstants.EMAIL;
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse_EMAIL = mobileAppBO.GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);
                List<AlertTemplateDynamicContent> DynamicTemplate = new List<AlertTemplateDynamicContent>(){
                new AlertTemplateDynamicContent ("[client name]",  ObjBillingSaveRequest.objBilling.CustomerName  ),
                new AlertTemplateDynamicContent ("[Reason]", Remark)};
                // Send Email
                if (ObjSmsEmailRetrieveResponse_EMAIL != null)
                {
                    if (ObjSmsEmailRetrieveResponse_EMAIL.lstSmsEmailDetails != null)
                    {
                        if (ObjSmsEmailRetrieveResponse_EMAIL.lstSmsEmailDetails.Count > 0)
                        {
                            for (int i = 0; i < ObjSmsEmailRetrieveResponse_EMAIL.lstSmsEmailDetails.Count; i++)
                            {
                                AlertUtiltityParameters alertUtiltityParameters_EMAIL = new AlertUtiltityParameters
                               (UIConstants.EMAIL, ObjBillingSaveRequest.objBilling.LoyaltyId, DynamicTemplate, ObjSmsEmailRetrieveResponse_EMAIL.lstSmsEmailDetails[i]);
                                alertUtiltityParameters_EMAIL.MailHeaderText = UIConstants.TRANSACTIONAL_HEADER;
                                var Result = SendAlertUtility.SendAlert(alertUtiltityParameters_EMAIL);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " SendRejectEmailToCustomer() " + ex.Message);
            }
        }

        public HSEResponse SaveHappinessStockExchangeDetails(HSERequest objHSERequest)
        {
            HSEResponse objHSEResponse = null;
            try
            {
                if (string.IsNullOrEmpty(objHSERequest.Base64Image))
                {
                    objHSEResponse = new HSEResponse();
                    objHSEResponse.ResponseCode = "006";
                    objHSEResponse.ReturnMessage = "Image is missing";
                    return objHSEResponse;
                }
                else
                {
                    string imageName = objHSERequest.MembershipID + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".png";
                    string FullPath = (HTTP_UPLOAD_FOLDER_PATH + PROGRAM_BAHAVIOUR_IMAGE).Replace("~", "");

                    bool res = UICommon.SaveBase64StringIntoFile(FullPath, objHSERequest.Base64Image, imageName);

                    objHSERequest.ImageName = imageName;
                }
                objHSEResponse = mobileAppBO.SaveHappinessStockExchangeDetails(objHSERequest);
            }
            catch (Exception ex)
            {
                objHSEResponse = new HSEResponse();
                objHSEResponse.ResponseCode = "008";
                objHSEResponse.ReturnMessage = "Internal server Error";
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + ":: SaveHappinessStockExchangeDetails() ::" + ex.Message);
            }
            return objHSEResponse;
        }

        public CustomerSaveChildMappingResponse SaveChildMapping(CustomerSaveChildMappingRequest objCustomerSaveChildMappingRequest)
        {
            CustomerSaveChildMappingResponse objCustomerSaveChildMappingResponse = null;
            try
            {
                string Password = UIConstants.VALUE_EMPTY;
                if (!string.IsNullOrEmpty(objCustomerSaveChildMappingRequest.Password))
                {
                    Password = objCustomerSaveChildMappingRequest.Password;
                    objCustomerSaveChildMappingRequest.Password = Security.EncryptPassword(objCustomerSaveChildMappingRequest.Password);
                }
                else
                {

                    if (DOMAIN == UIConstants.WAVIN)
                        Password = "654321";
                    else
                        Password = UICommon.GetRandomNumber();

                    objCustomerSaveChildMappingRequest.Password = Security.EncryptPassword(Password);
                }
                objCustomerSaveChildMappingRequest.Domain = DOMAIN;
                objCustomerSaveChildMappingResponse = mobileAppBO.SaveChildMapping(objCustomerSaveChildMappingRequest);
                if (objCustomerSaveChildMappingResponse != null && (DOMAIN == UIConstants.VOLVO || DOMAIN == UIConstants.REDINGTON))
                {
                    string[] Result = objCustomerSaveChildMappingResponse.ReturnMessage.Split('~').ToArray();
                    if (Result[0] == UIConstants.VALUE_ONE)
                    {
                        if (DOMAIN == UIConstants.VOLVO)
                            SendCustRegCredentials(objCustomerSaveChildMappingRequest.FirstName, objCustomerSaveChildMappingRequest.FirstName, Password, objCustomerSaveChildMappingRequest.MobileNumber, Result[1]);
                        else if (DOMAIN == UIConstants.REDINGTON && (objCustomerSaveChildMappingRequest.CustomerTypeId == 2 || objCustomerSaveChildMappingRequest.CustomerTypeId == 4)) // Only for Customer Type Staff and Supervisor Email,Push and SMS will trigger...
                        {
                            if (objCustomerSaveChildMappingRequest.ActionType == 1) // for Deactivate Account..
                            {
                                SendDeactiveNotificationForCustomer(UIConstants.SMS, objCustomerSaveChildMappingRequest.FirstName, objCustomerSaveChildMappingRequest.MobileNumber);
                                SendDeactiveNotificationForCustomer(UIConstants.EMAIL, objCustomerSaveChildMappingRequest.FirstName, objCustomerSaveChildMappingRequest.Email);
                            }
                            else
                            {
                                SendCustChildRegCredentials(objCustomerSaveChildMappingRequest.FirstName, objCustomerSaveChildMappingRequest.FirstName, Password, objCustomerSaveChildMappingRequest.MobileNumber, Result[1]);
                                SendCustChildRegCredentialsEmail(objCustomerSaveChildMappingRequest.FirstName, objCustomerSaveChildMappingRequest.FirstName, Password, objCustomerSaveChildMappingRequest.Email, Result[1]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objCustomerSaveChildMappingResponse = new CustomerSaveChildMappingResponse();
                objCustomerSaveChildMappingResponse.ReturnMessage = ex.Message;
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " SaveChildMapping() : " + ex.Message);
            }

            return objCustomerSaveChildMappingResponse;
        }

        public CustParentChildMappingRetriveResponse GetCustParentChildMappingDetails(CustParentChildMappingRetriveRequest objCustParentChildMappingRetriveRequest)
        {
            CustParentChildMappingRetriveResponse objCustParentChildMappingRetriveResponse = null;
            try
            {
                objCustParentChildMappingRetriveResponse = mobileAppBO.GetCustParentChildMappingDetails(objCustParentChildMappingRetriveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetCustParentChildMappingDetails() : " + ex.Message);
            }
            return objCustParentChildMappingRetriveResponse;
        }

        public ProgramBehaviourResponse GetProgramBehaviourCalData(ProgramBehaviourRequest objProgramBehaviourRequest)
        {
            ProgramBehaviourResponse objProgramBehaviourResponse = null;
            try
            {
                objProgramBehaviourResponse = mobileAppBO.GetProgramBehaviourCalData(objProgramBehaviourRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetProgramBehaviourCalData() : " + ex.Message);
            }

            return objProgramBehaviourResponse;
        }

        public ProgramBehaviourResponse GetAndUpdateScratchCardNotification(ProgramBehaviourRequest objProgramBehaviourRequest)
        {
            ProgramBehaviourResponse objProgramBehaviourResponse = null;
            try
            {
                objProgramBehaviourResponse = mobileAppBO.GetAndUpdateScratchCardNotification(objProgramBehaviourRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetAndUpdateScratchCardNotification() : " + ex.Message);
            }

            return objProgramBehaviourResponse;
        }


        public bool SendLoginCredentialsToCust(string UserName, string CustomerName, string MobileNumber)        //SMS
        {
            bool result = default(bool);
            try
            {
                UserRetrieveRequest_Loyalty userRetrieveRequest = new UserRetrieveRequest_Loyalty();
                userRetrieveRequest.UserName = UserName;
                userRetrieveRequest.Password = string.Empty;
                userRetrieveRequest.UserId = -1;
                userRetrieveRequest.ActionType = UPDATE_CHANGED_PASSWORD;
                string RandomPin_OTP = UICommon.GetRandomNumber();
                userRetrieveRequest.Password = RandomPin_OTP;
                userRetrieveRequest.MerchantUserName = MERCHANT_USERNAME;
                UserLoginRetrieveResponse userRetrieveResponse = CheckIsAuthenticatedMobileApp(userRetrieveRequest);
                if (userRetrieveResponse != null && userRetrieveResponse.UserList.Count > 0)
                {
                    if (userRetrieveResponse.UserList[0].Result == 1)
                    {
                        string UsrName = userRetrieveResponse.UserList[0].UserName;
                        SendCustRegCredentials(CustomerName, CustomerName, RandomPin_OTP, MobileNumber, UsrName);
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception ex)
            {
                result = false;
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " SendLoginCredentialsToCust() " + ex.Message);
            }
            return result;
        }

        public GetScratchCodeResponseJson GetQRCodeStatus(GetScratchCodeRequestJson ObjGetScratchCodeRequest)
        {
            GetScratchCodeResponseJson objGetScratchCodeResponse = null;
            try
            {
                objGetScratchCodeResponse = mobileAppBO.GetQRCodeStatus(ObjGetScratchCodeRequest);

            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " SendLoginCredentialsToCust() " + ex.Message);
            }
            return objGetScratchCodeResponse;
        }


        public GetScratchCodeResponseJson GetLatitudeLongitudeAddressDetails(GetScratchCodeRequestJson ObjGetScratchCodeRequest)
        {
            GetScratchCodeResponseJson objGetScratchCodeResponse = null;
            try
            {
                objGetScratchCodeResponse = mobileAppBO.GetLatitudeLongitudeAddressDetails(ObjGetScratchCodeRequest);

            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetLatitudeLongitudeAddressDetails() " + ex.Message);
            }
            return objGetScratchCodeResponse;
        }

        public TransactionApprovalRetrieveResponse GetPurchaseRequestDetailsList(TransactionApprovalRetrieveRequest ObjTransactionApprovalRetrieveRequest)
        {
            TransactionApprovalRetrieveResponse objGetPurchaseRequestDetailsList = null;
            try
            {
                objGetPurchaseRequestDetailsList = mobileAppBO.GetPurchaseRequestDetailsList(ObjTransactionApprovalRetrieveRequest);

            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetPurchaseRequestDetailsList() " + ex.Message);
            }
            return objGetPurchaseRequestDetailsList;
        }

        public TransactionApprovalRetrieveResponse SaveApprovePurchaseRequestDetail(TransactionApprovalRetrieveRequest ObjTransactionApprovalRetrieveRequest)
        {
            TransactionApprovalRetrieveResponse objGetPurchaseRequestDetailsList = null;
            try
            {
                ObjTransactionApprovalRetrieveRequest.Domain = DOMAIN;
                objGetPurchaseRequestDetailsList = mobileAppBO.SaveApprovePurchaseRequestDetail(ObjTransactionApprovalRetrieveRequest);
                if (objGetPurchaseRequestDetailsList != null)
                {
                    objGetPurchaseRequestDetailsList.ReturnValue = Convert.ToInt32(objGetPurchaseRequestDetailsList.ReturnMessage.Split('~')[0]);
                    if (objGetPurchaseRequestDetailsList.ReturnMessage.Split('~')[0] == UIConstants.VALUE_ONE)
                    {
                        if (ObjTransactionApprovalRetrieveRequest.ApprovalStatusID == 1 && objGetPurchaseRequestDetailsList.lstTransactionApprovalDetails != null && objGetPurchaseRequestDetailsList.lstTransactionApprovalDetails.Count > 0)
                        {
                            SendPointsAccumulationSms(MERCHANT_USERNAME, objGetPurchaseRequestDetailsList.lstTransactionApprovalDetails[0].MemberName, objGetPurchaseRequestDetailsList.lstTransactionApprovalDetails[0].Mobile, objGetPurchaseRequestDetailsList.lstTransactionApprovalDetails[0].RewardPoints.ToString(), objGetPurchaseRequestDetailsList.lstTransactionApprovalDetails[0].PointBalance.ToString());
                        }
                        else if (ObjTransactionApprovalRetrieveRequest.ApprovalStatusID == -1)
                        {
                            BillingSaveRequest objBillingSaveRequest = new BillingSaveRequest();
                            //SendRejectEmailToCustomer(ObjTransactionApprovalRetrieveRequest.ApprovalRemarks ,
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " SaveApprovePurchaseRequestDetail() " + ex.Message);
            }
            return objGetPurchaseRequestDetailsList;
        }

        public BillingSaveResponse SaveVolvoTransactionDetails(BillingSaveRequest objBillingSaveRequest)
        {
            BillingSaveResponse objBillingSaveResponse = null;

            try
            {
                if (!string.IsNullOrEmpty(objBillingSaveRequest.objBilling.Image))
                {
                    string ImagePath = (HTTP_UPLOAD_FOLDER_PATH + PROGRAM_BAHAVIOUR_IMAGE).Replace("~", "");
                    string imageName = objBillingSaveRequest.objBilling.LoyaltyId + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".png";
                    if (!UICommon.SaveBase64StringIntoFile(ImagePath, objBillingSaveRequest.objBilling.Image, imageName))
                    {
                        objBillingSaveResponse = new BillingSaveResponse
                        {
                            ReturnMessage = "Problem occured while processing image.",
                            ReturnValue = -1
                        };
                        return objBillingSaveResponse;
                    }
                    objBillingSaveRequest.objBilling.ImagePath = imageName;
                    objBillingSaveRequest.objBilling.Image = string.Empty;
                }
                if (!string.IsNullOrEmpty(objBillingSaveRequest.objBilling.Video))
                {
                    string VideoPath = (HTTP_UPLOAD_FOLDER_PATH + REVIEW_VIDEO_PATH).Replace("~", ""); ;
                    string VideoName = objBillingSaveRequest.objBilling.LoyaltyId + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".mp4";
                    if (!UICommon.SaveBase64StringIntoFile(VideoPath, objBillingSaveRequest.objBilling.Video, VideoName))
                    {
                        objBillingSaveResponse = new BillingSaveResponse
                        {
                            ReturnMessage = "Problem occured while processing video.",
                            ReturnValue = -1
                        };
                        return objBillingSaveResponse;
                    }
                    objBillingSaveRequest.objBilling.VideoPath = VideoName;
                    objBillingSaveRequest.objBilling.Video = string.Empty;
                }
                objBillingSaveResponse = mobileAppBO.SaveVolvoTransactionDetails(objBillingSaveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + ":: SaveVolvoTransactionDetails() ::" + ex.Message);
            }
            return objBillingSaveResponse;
        }

        public ProductSaveResponse SaveEGiftRequest_MSSC(ProductSaveRequest objProductSaveRequest)
        {
            ProductSaveResponse objProductSaveResponse = null;
            try
            {
                string FullPath = string.Empty, FileName = string.Empty;
                FullPath = HTTP_UPLOAD_FOLDER_PATH + PROFILE_PIC_FOLDER_PATH;
                //Display Image
                if (objProductSaveRequest.BehaviorID != 3)
                {
                    FileName = objProductSaveRequest.LoyaltyID + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".png";
                    System.Drawing.Image CustomerImage = UICommon.Base64ToImageConverter(objProductSaveRequest.ProductSaveDetails.ProductImage);

                    CustomerImage.Save((FullPath.Replace("~", "") + FileName), ImageFormat.Png);
                    objProductSaveRequest.ProductSaveDetails.ProductImage = PROFILE_PIC_FOLDER_PATH.Replace("\\", "/") + FileName;

                }
                else
                {
                    FileName = "Assessment_Certificate" + objProductSaveRequest.LoyaltyID + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".png";
                    System.Drawing.Image Assessment_Certificate = UICommon.Base64ToImageConverter(objProductSaveRequest.AssessmentCertificate);

                    Assessment_Certificate.Save((FullPath.Replace("~", "") + FileName), ImageFormat.Png);
                    objProductSaveRequest.AssessmentCertificate = PROFILE_PIC_FOLDER_PATH.Replace("\\", "/") + FileName;

                }
                objProductSaveResponse = mobileAppBO.SaveEGiftRequest_MSSC(objProductSaveRequest);

                if (objProductSaveResponse != null)
                {
                    string[] arg = objProductSaveResponse.ReturnMessage.Split('~');
                    string ReturnValue = arg[0];

                }

            }
            catch (Exception ex)
            {

                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " : SaveEGiftRequest_MSSC() : " + ex.Message);
            }

            return objProductSaveResponse;
        }

        public ProductSaveResponse SaveEGiftRequest_Wavin(ProductSaveRequest objProductSaveRequest)
        {
            ProductSaveResponse objProductSaveResponse = null;
            try
            {
                string FullPath = string.Empty, FileName = string.Empty;
                FullPath = HTTP_UPLOAD_FOLDER_PATH + PROFILE_PIC_FOLDER_PATH;
                //Display Image
                if (objProductSaveRequest.ProductSaveDetails.ProductImage != null)
                {
                    FileName = objProductSaveRequest.LoyaltyID + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".png";
                    System.Drawing.Image CustomerImage = UICommon.Base64ToImageConverter(objProductSaveRequest.ProductSaveDetails.ProductImage);

                    CustomerImage.Save((FullPath.Replace("~", "") + FileName), ImageFormat.Png);
                    objProductSaveRequest.ProductSaveDetails.ProductImage = PROFILE_PIC_FOLDER_PATH.Replace("\\", "/") + FileName;

                }
                objProductSaveResponse = mobileAppBO.SaveEGiftRequest_Wavin(objProductSaveRequest);

                if (objProductSaveResponse != null)
                {
                    string[] arg = objProductSaveResponse.ReturnMessage.Split('~');
                    string ReturnValue = arg[0];

                }

            }
            catch (Exception ex)
            {

                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " : SaveEGiftRequest_Wavin() : " + ex.Message);
            }

            return objProductSaveResponse;
        }

        public ProductSaveResponse SaveClaimRequestAtomberg(ProductSaveRequest objProductSaveRequest)
        {
            ProductSaveResponse objProductSaveResponse = null;
            try
            {
                string FullPath = string.Empty, FileName = string.Empty;
                FullPath = HTTP_UPLOAD_FOLDER_PATH + PROFILE_PIC_FOLDER_PATH;
                //Display Image
                if (objProductSaveRequest.ProductSaveDetails.ProductImage != null)
                {
                    FileName = objProductSaveRequest.LoyaltyID + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".png";
                    System.Drawing.Image CustomerImage = UICommon.Base64ToImageConverter(objProductSaveRequest.ProductSaveDetails.ProductImage);

                    CustomerImage.Save((FullPath.Replace("~", "") + FileName), ImageFormat.Png);
                    objProductSaveRequest.ProductSaveDetails.ProductImage = PROFILE_PIC_FOLDER_PATH.Replace("\\", "/") + FileName;

                }
                objProductSaveResponse = mobileAppBO.SaveClaimRequestAtomberg(objProductSaveRequest);

                if (objProductSaveResponse != null)
                {
                    string[] arg = objProductSaveResponse.ReturnMessage.Split('~');
                    string ReturnValue = arg[0];

                }

            }
            catch (Exception ex)
            {

                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " : SaveClaimRequestAtomberg() : " + ex.Message);
            }

            return objProductSaveResponse;
        }


        public ProductRetriveResponseAPI BindProductDetailsForMobile(ProductRetriveRequest objProductRetriveRequest)
        {
            ProductRetriveResponseAPI objProductRetriveResponse = null;
            try
            {
                objProductRetriveResponse = mobileAppBO.BindProductDetailsForMobile(objProductRetriveRequest);

            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetPurchaseRequestDetailsList() " + ex.Message);
            }
            return objProductRetriveResponse;
        }

        public ProductSaveResponse SaveCustomerCartDetailsMobile(ProductSaveRequest objProductSaveRequest)
        {
            ProductSaveResponse objProductSaveResponse = null;
            try
            {
                objProductSaveResponse = mobileAppBO.SaveCustomerCartDetailsMobile(objProductSaveRequest);

            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " SaveCustomerCartDetailsMobile() " + ex.Message);
            }
            return objProductSaveResponse;
        }


        public ProductSaveResponse GetProductStockByLoyaltyID(string loyaltyId, string searchText)
        {
            ProductSaveResponse objProductSaveResponse = null;
            try
            {
                objProductSaveResponse = mobileAppBO.GetProductStockByLoyaltyID(loyaltyId, searchText);

            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetProductStockByLoyaltyID() " + ex.Message);
            }
            return objProductSaveResponse;
        }

        public CustomerSaveChildMappingResponse SaveCustomerToCustomerHierarchyMapping(CustomerSaveChildMappingRequest objCustomerSaveChildMappingRequest)
        {
            CustomerSaveChildMappingResponse objCustomerSaveChildMappingResponse = null;
            try
            {
                objCustomerSaveChildMappingResponse = mobileAppBO.SaveCustomerToCustomerHierarchyMapping(objCustomerSaveChildMappingRequest);

            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " SaveCustomerToCustomerHierarchyMapping() " + ex.Message);
            }
            return objCustomerSaveChildMappingResponse;
        }

        public ReferralConvRetriveResponse GetReferenceAndEntrollmentBonus(ReferralConvRetriveRequest objReferralConvRetriveRequest)
        {
            ReferralConvRetriveResponse objReferralConvRetriveResponse = null;
            try
            {
                objReferralConvRetriveResponse = mobileAppBO.GetReferenceAndEntrollmentBonus(objReferralConvRetriveRequest);

            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetReferenceAndEntrollmentBonus() " + ex.Message);
            }
            return objReferralConvRetriveResponse;
        }
        public CustomerSaveResponse SaveCustomerDetails_MSSC(CustomerSaveRequest ObjCustomerSaveRequest)
        {
            CustomerSaveResponse objCustomerSaveResponse = null;
            try
            {
                string FullPath = string.Empty, FileName = string.Empty;
                FullPath = HTTP_UPLOAD_FOLDER_PATH + "~/UploadFiles/IdentificationProof/";
                //Display Image
                if (ObjCustomerSaveRequest.ObjCustomer.TradeLicence != null)
                {
                    FileName = DateTime.Now.ToString("ddMMyyyyhhmmss") + "_TradeLicence_" + ".png";
                    System.Drawing.Image CustomerImage = UICommon.Base64ToImageConverter(ObjCustomerSaveRequest.ObjCustomer.TradeLicence);

                    CustomerImage.Save((FullPath.Replace("~", "") + FileName), ImageFormat.Png);
                    ObjCustomerSaveRequest.ObjCustomer.TradeLicence = "~/UploadFiles/IdentificationProof/".Replace("\\", "/") + FileName;

                }
                ObjCustomerSaveRequest.ObjCustomer.Password = Security.EncryptPassword(Convert.ToString(ObjCustomerSaveRequest.ObjCustomer.Password));
                objCustomerSaveResponse = mobileAppBO.SaveCustomerDetails_MSSC(ObjCustomerSaveRequest);

                if (objCustomerSaveResponse != null)
                {
                    string[] arg = objCustomerSaveResponse.ReturnMessage.Split('~');
                    string ReturnValue = arg[0];
                    SendCustomerEmail(ObjCustomerSaveRequest.ObjCustomer.FirstName, "", "", "", ObjCustomerSaveRequest.ObjCustomer.Email, ObjCustomerSaveRequest.MerchantType);

                }

            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " SaveCustomerDetails_MSSC() " + ex.Message);
            }
            return objCustomerSaveResponse;
        }
        private void SendCustomerEmail(string CustomerName, string PlainPassword, string LoyaltyId, string EnrollmentPoints, string EmailID, string MerchantUserName)
        {
            try
            {

                string CustomerPwdEncrypt = UICommon.Encrypt(PlainPassword);

                SmsEmail ObjSmsEmail = new SmsEmail();
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.WelcomeAccountactivation);
                ObjSmsEmailRetrieveRequest.Type = UIConstants.EMAIL;
                ObjSmsEmailRetrieveRequest.UserName = MerchantUserName;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = EmailID;

                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse = mobileAppBO.GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);
                if (ObjSmsEmailRetrieveResponse != null && ObjSmsEmailRetrieveResponse.lstSmsEmailDetails != null && ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count > 0)
                {
                    for (int i = 0; i < ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count; i++)
                    {

                        List<AlertTemplateDynamicContent> DynamicTemplate = new List<AlertTemplateDynamicContent>()
                    {new AlertTemplateDynamicContent(ClientName, CustomerName)
                        };

                        AlertUtiltityParameters alertUtiltityParameters = new AlertUtiltityParameters
                       (UIConstants.EMAIL, EmailID, DynamicTemplate, ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i]);
                        alertUtiltityParameters.MailHeaderText = UIConstants.WELCOME_HEADER;
                        var Result = SendAlertUtility.SendAlert(alertUtiltityParameters);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " SendCustomerEmail() " + ex.Message);
            }
        }


        public RetailerBondingResponse GetRetailerBondingDetails(RetailerBondingRequest objRetailerBondingRequest)
        {
            RetailerBondingResponse objRetailerBondingResponse = null;
            try
            {
                objRetailerBondingResponse = mobileAppBO.GetRetailerBondingDetails(objRetailerBondingRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetRetailerBondingDetails() " + ex.Message);
            }
            return objRetailerBondingResponse;
        }

        public CustomerPointDetailsRetriveResponse GetCustPointDetails_ATMB(CustomerPointDetailsRetriveRequest objCustomerPointDetailsRetriveRequest)
        {
            CustomerPointDetailsRetriveResponse objCustomerPointDetailsRetriveResponse = null;
            try
            {
                objCustomerPointDetailsRetriveResponse = mobileAppBO.GetCustPointDetails_ATMB(objCustomerPointDetailsRetriveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " : GetCustPointDetails_ATMB() : " + ex.Message);
            }
            return objCustomerPointDetailsRetriveResponse;
        }

        public CustParentChildMappingRetriveResponse GetMappedParentChildEnrollAndTranDetails(CustParentChildMappingRetriveRequest objCustParentChildMappingRetriveRequest)
        {
            CustParentChildMappingRetriveResponse objCustParentChildMappingRetriveResponse = null;
            try
            {
                if (!string.IsNullOrEmpty(objCustParentChildMappingRetriveRequest.JDateFrom) && (objCustParentChildMappingRetriveRequest.JDateFrom != Convert.ToString("1/1/0001")))
                    objCustParentChildMappingRetriveRequest.DateFrom = Convert.ToDateTime(objCustParentChildMappingRetriveRequest.JDateFrom);
                if (!string.IsNullOrEmpty(objCustParentChildMappingRetriveRequest.JDateTo) && (objCustParentChildMappingRetriveRequest.JDateTo != Convert.ToString("1/1/0001")))
                    objCustParentChildMappingRetriveRequest.DateTo = Convert.ToDateTime(objCustParentChildMappingRetriveRequest.JDateTo);
                objCustParentChildMappingRetriveResponse = mobileAppBO.GetMappedParentChildEnrollAndTranDetails(objCustParentChildMappingRetriveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " : GetMappedParentChildEnrollAndTranDetails() : " + ex.Message);
            }
            return objCustParentChildMappingRetriveResponse;
        }

        public ContactCenterSaveResponse SaveReferralDetails(ContactCenterSaveRequest objContactCenterSaveRequest)
        {
            ContactCenterSaveResponse objContactCenterSaveResponse = null;
            try
            {
                objContactCenterSaveResponse = mobileAppBO.SaveReferralDetails(objContactCenterSaveRequest);
                CustomerRetrieveRequest objCustomerRetrieveRequest = new CustomerRetrieveRequest();
                if (objContactCenterSaveResponse.ReturnMessage == UIConstants.VALUE_ONE)
                {

                    if (DOMAIN == UIConstants.GVLC)
                    {
                        objCustomerRetrieveRequest.ActorId = objContactCenterSaveRequest.ActorId;
                        objCustomerRetrieveRequest.ActionType = 2;
                        CustomerRetrieveResponse objCustomerRetrieveResponseSms = mobileAppBO.GetUserDetailsForPushNotification(objCustomerRetrieveRequest);

                        if (objCustomerRetrieveResponseSms != null && objCustomerRetrieveResponseSms.lstCustomer != null && objCustomerRetrieveResponseSms.lstCustomer.Count > 0)
                        {
                            SendReferralAlert(objContactCenterSaveRequest.ObjContactCenterDetails.RefereeMobileNo, objContactCenterSaveRequest.ObjContactCenterDetails.RefereeName, objCustomerRetrieveResponseSms.lstCustomer[0].ReferralCode, objCustomerRetrieveResponseSms.lstCustomer[0].FirstName);
                        }

                    }
                    else
                    {
                        objCustomerRetrieveRequest.ActorId = objContactCenterSaveRequest.ActorId;
                        objCustomerRetrieveRequest.ActionType = 1;
                        CustomerRetrieveResponse objCustomerRetrieveResponse = mobileAppBO.GetUserDetailsForPushNotification(objCustomerRetrieveRequest);

                        if (objCustomerRetrieveResponse != null && objCustomerRetrieveResponse.lstCustomer != null && objCustomerRetrieveResponse.lstCustomer.Count > 0)
                        {
                            foreach (var name in objCustomerRetrieveResponse.lstCustomer)
                            {
                                SendGetUserDetailsForPushNotification(name.UserName, name.Mobile, name.FirstName, name.CustomerType);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " : SaveReferralDetails() : " + ex.Message);
            }
            return objContactCenterSaveResponse;
        }

        private void SendGetUserDetailsForPushNotification(string UserName, string Mobile, string FirstName, string CustomerType = "")
        {
            try
            {
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse = new SmsEmailRetrieveResponse();

                SmsEmail ObjSmsEmail = new SmsEmail();
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();

                ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.REFERRAL_REQUEST_HAS_BEEN_SUBMITTED);

                ObjSmsEmailRetrieveRequest.UserName = UserName;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = Mobile;
                ObjSmsEmailRetrieveRequest.Type = "SMS";
                ObjSmsEmailRetrieveResponse = mobileAppBO.GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);
                for (int i = 0; i < ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count; i++)
                {
                    List<AlertTemplateDynamicContent> DynamicTemplateEmail = new List<AlertTemplateDynamicContent>()
                {
                    new AlertTemplateDynamicContent("[MEMBER]", FirstName),

                    new AlertTemplateDynamicContent("[MEMBER TYPE]", CustomerType),

                };

                    AlertUtiltityParameters alertUtiltityParameterEmail = new AlertUtiltityParameters
                                  (ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i].Type, Mobile, DynamicTemplateEmail, ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i]);

                    var MailResult = SendAlertUtility.SendAlert(alertUtiltityParameterEmail);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "SendEmailToCustomerForPostedQuery() : " + ex.Message);
            }
        }

        private void SendReferralAlert(string Mobile, string FirstName, string ReferralCode, string ClientName)
        {
            try
            {
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse = new SmsEmailRetrieveResponse();

                SmsEmail ObjSmsEmail = new SmsEmail();
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();

                ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.REFERRAL_REQUEST_HAS_BEEN_SUBMITTED);

                ObjSmsEmailRetrieveRequest.UserName = MERCHANT_USERNAME;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = Mobile;
                ObjSmsEmailRetrieveRequest.Type = "SMS";
                ObjSmsEmailRetrieveResponse = mobileAppBO.GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);
                for (int i = 0; i < ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count; i++)
                {
                    List<AlertTemplateDynamicContent> DynamicTemplateEmail = new List<AlertTemplateDynamicContent>()
                {
                    new AlertTemplateDynamicContent("[Referee_Name]", FirstName),
                     new AlertTemplateDynamicContent("[Member_Name]", ClientName),
                    new AlertTemplateDynamicContent("[Referal_code]", ReferralCode)

                };

                    AlertUtiltityParameters alertUtiltityParameterEmail = new AlertUtiltityParameters
                                  (ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i].Type, Mobile, DynamicTemplateEmail, ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i]);

                    var MailResult = SendAlertUtility.SendAlert(alertUtiltityParameterEmail);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "SendReferralAlert() : " + ex.Message);
            }
        }
        private void SendVoucherAlert(CatalogueSaveRequest objCatalogueSaveRequest, string TemplateName)
        {
            try
            {
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                ObjSmsEmailRetrieveRequest.TemplateName = TemplateName;
                ObjSmsEmailRetrieveRequest.Type = UIConstants.SMS;
                ObjSmsEmailRetrieveRequest.UserName = MERCHANT_USERNAME;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = objCatalogueSaveRequest.ReceiverMobile;
                SmsEmailRetrieveResponse ObjSmsRetrieveResponse = mobileAppBO.GetEmailSmsTemplate(ObjSmsEmailRetrieveRequest);

                ObjSmsEmailRetrieveRequest.Type = UIConstants.EMAIL;

                SmsEmailRetrieveResponse ObjEmailRetrieveResponse = mobileAppBO.GetEmailSmsTemplate(ObjSmsEmailRetrieveRequest);
                if (ObjSmsRetrieveResponse != null || ObjEmailRetrieveResponse != null)
                {
                    List<AlertTemplateDynamicContent> DynamicTemplate = new List<AlertTemplateDynamicContent>()
                            {

                                new AlertTemplateDynamicContent("[MemberName]",objCatalogueSaveRequest.ReceiverName),
                                new AlertTemplateDynamicContent("[VoucherCardNum]",UICommon.Decrypt(objCatalogueSaveRequest.VoucherCardNumber)),
                                new AlertTemplateDynamicContent("[VoucherCardPin]",UICommon.Decrypt(objCatalogueSaveRequest.VoucherCardPin)),
                                new AlertTemplateDynamicContent("[VoucherValidity]",objCatalogueSaveRequest.expiry_date)
                            };

                    if (ObjSmsRetrieveResponse != null)
                    {
                        for (int i = 0; i < ObjSmsRetrieveResponse.lstSmsEmailDetails.Count; i++)
                        {
                            AlertUtiltityParameters alertUtiltityParametersSMS = new AlertUtiltityParameters
                            (ObjSmsRetrieveResponse.lstSmsEmailDetails[i].Type, objCatalogueSaveRequest.ReceiverEmail, DynamicTemplate, ObjSmsRetrieveResponse.lstSmsEmailDetails[i]);
                            alertUtiltityParametersSMS.IsGetRequest = true;
                            if (DOMAIN == UIConstants.RSM)
                                alertUtiltityParametersSMS.VendorMethodName = SMSVendorMethodEnum.BulkPushMyToday;
                            var Result = SendAlertUtility.SendAlert(alertUtiltityParametersSMS);
                        }
                    }

                    if (ObjEmailRetrieveResponse != null)
                    {
                        for (int i = 0; i < ObjEmailRetrieveResponse.lstSmsEmailDetails.Count; i++)
                        {
                            AlertUtiltityParameters alertUtiltityParametersEmail = new AlertUtiltityParameters
                            (UIConstants.EMAIL, objCatalogueSaveRequest.ReceiverEmail, DynamicTemplate, ObjEmailRetrieveResponse.lstSmsEmailDetails[i]);


                            var Result = SendAlertUtility.SendAlert(alertUtiltityParametersEmail);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " SendVoucherAlert() " + ex.Message);
            }
        }

        private decimal GetWalletBalance(int UserId, int MerchantId, string ApiKey, string APISecretKey)
        {
            decimal WalletAmount = 0;
            try
            {
                if (VoucherVendor == UIConstants.SURE_GIFT)
                {
                    string BalanceAPI = string.Empty;
                    if (MerchantId == 1)
                    {
                        BalanceAPI = SURE_GIFT_BALANCE_API_K;
                    }
                    else
                    {
                        BalanceAPI = SURE_GIFT_BALANCE_API_N;
                    }
                    Skipper.Common.Utility.HttpRequestUtility.HttpRequestArguments httpRequestArguments = new Skipper.Common.Utility.HttpRequestUtility.HttpRequestArguments(BalanceAPI, "", "application/json", false, "");
                    httpRequestArguments.Headers = new WebHeaderCollection();
                    httpRequestArguments.Headers.Add("Authorization", ApiKey);
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                    string ResponseString = Skipper.Common.Utility.HttpRequestUtility.HttpRequest.MakeGetRequest(httpRequestArguments);

                    if (!string.IsNullOrEmpty(ResponseString))
                    {
                        JObject joResponse = JObject.Parse(ResponseString);

                        string Balance = joResponse["Data"]["WalletBalance"].ToString();

                        var str = Balance.Replace("N", "");
                        str = Balance.Replace("Ksh", "");
                        WalletAmount = Convert.ToDecimal(str.Trim());

                    }
                }
                else
                {
                    VendorRetrieveRequest ObjVendorRetrieveRequest = new VendorRetrieveRequest();
                    ObjVendorRetrieveRequest.ActionType = (int)Enumeration_Loyalty.ActionType.ActiveListForDDL;
                    ObjVendorRetrieveRequest.ActorId = UserId;
                    ObjVendorRetrieveRequest.objVendorDetails = new Vendor();
                    ObjVendorRetrieveRequest.objVendorDetails.VendorId = VoucherVendorId;
                    VendorRetrieveResponse ObjVendorRetrieveResponse = mobileAppBO.GetVendorDetails(ObjVendorRetrieveRequest);
                    if (ObjVendorRetrieveResponse != null)
                    {
                        WalletAmount = ObjVendorRetrieveResponse.objVendorList.SingleOrDefault(x => x.WalletBalance != 0).WalletBalance;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetWalletBalance() " + ex.Message);
            }
            return WalletAmount;
        }

        public ELearningAnalysisResponse GetELearningAnalysisAndCirtificateDetails(ELearningAnalysisRequest objELearningAnalysisRequest)
        {
            ELearningAnalysisResponse objELearningAnalysisResponse = null;
            try
            {
                objELearningAnalysisResponse = mobileAppBO.GetELearningAnalysisAndCirtificateDetails(objELearningAnalysisRequest);
                objELearningAnalysisResponse.lstQuestionOptions[0].Certificate = GeneratedCertificate(objELearningAnalysisResponse.lstQuestionOptions[0].CustomerName, objELearningAnalysisResponse.lstQuestionOptions[0].Title, objELearningAnalysisResponse.lstQuestionOptions[0].Chapter, Convert.ToString(objELearningAnalysisResponse.lstQuestionOptions[0].Level), Convert.ToString(objELearningAnalysisResponse.lstQuestionOptions[0].PassingPercentage), objELearningAnalysisResponse.lstQuestionOptions[0].AssessmentDate);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " : GetELearningAnalysisAndCirtificateDetails() : " + ex.Message);
            }
            return objELearningAnalysisResponse;
        }

        public Byte[] GeneratedCertificate(string Name, string title, string Chapter, string level, string perchantage, string date)
        {
            Byte[] bytes = null;
            try
            {
                ReportViewer rdlReportViewer = new ReportViewer();
                rdlReportViewer.LocalReport.DataSources.Clear();
                rdlReportViewer.LocalReport.ReportPath = rdlcReportPath;
                rdlReportViewer.LocalReport.EnableHyperlinks = true;
                rdlReportViewer.LocalReport.Refresh();
                ReportParameter p1 = new ReportParameter("Name", Name);
                ReportParameter p2 = new ReportParameter("title", title);
                ReportParameter p3 = new ReportParameter("Chapter", Chapter);
                ReportParameter p4 = new ReportParameter("level", level);
                ReportParameter p5 = new ReportParameter("perchantage", perchantage);
                ReportParameter p6 = new ReportParameter("date", date);
                rdlReportViewer.LocalReport.SetParameters(new ReportParameter[] { p1, p2, p3, p4, p5, p6 });

                //Export
                Warning[] warnings;
                string[] streamIds;
                string mimeType = string.Empty;
                string encoding = string.Empty;
                string extension = string.Empty;
                string fileName = "AssesmentCertificate" + "_" + System.DateTime.Now.ToString("ddMMyyyy_hhmm");
                bytes = rdlReportViewer.LocalReport.Render("pdf", null, out mimeType, out encoding, out extension, out streamIds, out warnings);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GeneratedCertificate() " + ex.Message);
            }
            return bytes;
        }

        public AssessmentResponse GetAssessmentQuestions(AssessmentRequest ObjAssessmentRequest)
        {
            AssessmentResponse objAssessmentResponse = null;
            try
            {

                objAssessmentResponse = new MobileAppBO().GetAssessmentQuestions(ObjAssessmentRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetAssessmentQuestions() " + ex.Message);
            }
            return objAssessmentResponse;
        }

        public ExamSaveResponse SaveAssessmentDetails(ExamSaveRequest objExamSaveRequest)
        {
            ExamSaveResponse objExamSaveResponse = null;
            try
            {

                objExamSaveResponse = new MobileAppBO().SaveAssessmentDetails(objExamSaveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " SaveAssessmentDetails() " + ex.Message);
            }
            return objExamSaveResponse;
        }

        public ELearnChapterResponse GetELearnChapterDetails(ELearnChapterRequest objELearnChapterRequest)
        {
            ELearnChapterResponse objELearnChapterResponse = null;
            try
            {

                objELearnChapterResponse = new MobileAppBO().GetELearnChapterDetails(objELearnChapterRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetELearnChapterDetails() " + ex.Message);
            }
            return objELearnChapterResponse;
        }


        public ElearningRetrieveResponseJson GetELearningDetails(ElearningRetrieveRequestJson ObjElearningRetrieveRequest)
        {
            ElearningRetrieveResponseJson ObjElearningRetrieveResponse = null;
            try
            {
                ObjElearningRetrieveResponse = new MobileAppBO().GetELearningDetails(ObjElearningRetrieveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetELearningDetails() " + ex.Message);
            }
            return ObjElearningRetrieveResponse;
        }

        public MenuRetrieveResponse GetELearningMenuDetails(MenuRetrieveRequest ObjMenuRetrieveRequest)
        {
            MenuRetrieveResponse ObjMenuRetrieveResponse = null;
            try
            {
                ObjMenuRetrieveResponse = mobileAppBO.GetELearningMenuDetails(ObjMenuRetrieveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "GetELearningMenuDetails : " + ex.Message);
            }
            return ObjMenuRetrieveResponse;
        }

        public SceneResponse GetSceneDetails(SceneRequest objSceneRequest)
        {
            SceneResponse objSceneResponse = null;
            try
            {
                objSceneResponse = mobileAppBO.GetSceneDetails(objSceneRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetSceneDetails() " + ex.Message);
            }
            return objSceneResponse;
        }

        public AttributesRetrieveResponse GetAttributeDetails(AttributesRetrieveRequest objAttributesRetrieveRequest)
        {
            AttributesRetrieveResponse objAttributesRetrieveResponse = null;
            try
            {
                objAttributesRetrieveResponse = mobileAppBO.GetAttributeDetails(objAttributesRetrieveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "GetAttributeDetails : " + ex.Message);
            }
            finally
            {
            }
            return objAttributesRetrieveResponse;
        }
        public CustBankDetailsApprovalRetriveResponse GetCustomerBankDetails(CustBankDetailsApprovalRetriveRequest objCustBankDetailsApprovalRetriveRequest)
        {
            CustBankDetailsApprovalRetriveResponse ObjCustBankDetailsApprovalRetriveResponse = null;
            try
            {

                ObjCustBankDetailsApprovalRetriveResponse = mobileAppBO.GetCustomerBankDetails(objCustBankDetailsApprovalRetriveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetCustomerBankDetails() : " + ex.Message);
            }
            return ObjCustBankDetailsApprovalRetriveResponse;
        }
        public ImageGalleryResponce BindLandingImageList(ImageGalleryRequest objImageGalleryRequest)
        {
            ImageGalleryResponce objImageGalleryResponce = null;
            try
            {
                objImageGalleryResponce = mobileAppBO.BindLandingImageList(objImageGalleryRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " BindLandingImageList() : " + ex.Message);
            }
            return objImageGalleryResponce;
        }
        public CountryRetrieveResponse GetCountryDetails(CountryRetrieveRequest countryRetrieveRequest)
        {
            CountryRetrieveResponse countryRetrieveResponse = null;
            try
            {

                countryRetrieveResponse = mobileAppBO.GetCountryDetails(countryRetrieveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetCountryDetails : " + ex.Message);
            }
            finally { }
            return countryRetrieveResponse;
        }
        public StateRetrieveResponse GetStateDetails(StateRetrieveRequest stateRetrieveRequest)
        {
            StateRetrieveResponse stateRetrieveResponse = null;
            try
            {

                stateRetrieveResponse = mobileAppBO.GetStateDetails(stateRetrieveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetStateDetails : " + ex.Message);
            }
            finally { }
            return stateRetrieveResponse;
        }
        public DistrictRetrieveResponse GetDistrictDetails(DistrictRetrieveRequest ObjDistrictRetrieveRequest)
        {
            DistrictRetrieveResponse ObjDistrictRetrieveResponse = null;
            try
            {

                ObjDistrictRetrieveResponse = mobileAppBO.GetDistrictDetails(ObjDistrictRetrieveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + ":: GetDistrictDetails() ::" + ex.Message);
            }
            return ObjDistrictRetrieveResponse;
        }
        public CityRetrieveResponse GetCityDetails(CityRetrieveRequest cityRetrieveRequest)
        {
            CityRetrieveResponse CityRetrieveResponse = null;
            try
            {

                CityRetrieveResponse = mobileAppBO.GetCityDetails(cityRetrieveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetCityDetails : " + ex.Message);
            }
            finally { }
            return CityRetrieveResponse;
        }
        public QueryReplyRetrieveResponse SaveCustomerQueryTicket(QueryReplyRetrieveRequest objQueryReplyRetrieveRequest)
        {
            QueryReplyRetrieveResponse objQueryReplyRetrieveResponse = new QueryReplyRetrieveResponse();
            try
            {
                if (objQueryReplyRetrieveRequest.IsQueryFromMobile)
                {
                    string FullPath = string.Empty, FileName = string.Empty;
                    FullPath = HTTP_UPLOAD_FOLDER_PATH + PROFILE_PIC_FOLDER_PATH;
                    //Display Image
                    if (!string.IsNullOrEmpty(objQueryReplyRetrieveRequest.ImageUrl) && !string.IsNullOrEmpty(objQueryReplyRetrieveRequest.FileType))
                    {
                        FileName = objQueryReplyRetrieveRequest.LoyaltyID + "CustomerQuery" + DateTime.Now.ToString("ddMMyyyyhhmmss") + "." + objQueryReplyRetrieveRequest.FileType;
                        bool Result = UICommon.SaveBase64StringIntoFile(FullPath.Replace("~", ""), objQueryReplyRetrieveRequest.ImageUrl, FileName);
                        objQueryReplyRetrieveRequest.ImageUrl = PROFILE_PIC_FOLDER_PATH.Replace("\\", "/") + FileName;
                    }
                    else if (!string.IsNullOrEmpty(objQueryReplyRetrieveRequest.ImageUrl))
                    {
                        FileName = objQueryReplyRetrieveRequest.LoyaltyID + "CustomerQuery" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".png";
                        bool Result = UICommon.SaveBase64StringIntoFile(FullPath.Replace("~", ""), objQueryReplyRetrieveRequest.ImageUrl, FileName);
                        objQueryReplyRetrieveRequest.ImageUrl = PROFILE_PIC_FOLDER_PATH.Replace("\\", "/") + FileName;
                    }
                }

                objQueryReplyRetrieveResponse = mobileAppBO.SaveCustomerQueryTicket(objQueryReplyRetrieveRequest);
                if (objQueryReplyRetrieveRequest.IsQueryFromMobile)
                {
                    if (!string.IsNullOrEmpty(objQueryReplyRetrieveResponse.ReturnMessage))
                    {
                        string[] args = objQueryReplyRetrieveResponse.ReturnMessage.ToString().Split('~');
                        if (Convert.ToInt32(args[0]) > 0)
                        {
                            CustomerRetrieveRequest objCustomerRetrieveRequest = new CustomerRetrieveRequest();
                            objCustomerRetrieveRequest.ActorId = objQueryReplyRetrieveRequest.ActorId;
                            if (objQueryReplyRetrieveRequest.UserType == UIConstants.USER)
                            {
                                SendGetUserDetailsForPushNotification(args[2], objQueryReplyRetrieveRequest, objQueryReplyRetrieveRequest.LoyaltyID, objQueryReplyRetrieveRequest.Mobile, UIConstants.USER);
                            }
                            else if (objQueryReplyRetrieveRequest.UserType == UIConstants.CUSTOMER)
                            {
                                objCustomerRetrieveRequest.ActionType = 1;
                                CustomerRetrieveResponse objCustomerRetrieveResponse = mobileAppBO.GetUserDetailsForPushNotification(objCustomerRetrieveRequest);
                                if (objCustomerRetrieveResponse != null && objCustomerRetrieveResponse.lstCustomer != null && objCustomerRetrieveResponse.lstCustomer.Count > 0)
                                {
                                    foreach (var name in objCustomerRetrieveResponse.lstCustomer)
                                    {
                                        SendGetUserDetailsForPushNotification(args[2], objQueryReplyRetrieveRequest, name.UserName, name.Mobile, UIConstants.CUSTOMER, name.CustomerType);
                                    }
                                }

                            }
                            //SendGetUserDetailsForPushNotification(args[2], objQueryReplyRetrieveRequest, objQueryReplyRetrieveRequest.LoyaltyID, objQueryReplyRetrieveRequest.Mobile, UIConstants.CUSTOMER);
                            SendEmailToCustomerForPostedQuery(args[2], objQueryReplyRetrieveRequest);
                            SendEmailToMerchantForPostedQuery(args[2], objQueryReplyRetrieveRequest);
                            if (DOMAIN == UIConstants.MILLER_CLUB)
                            {
                                CustomerRetrieveRequest ObjCustomerRetrieveRequest = new CustomerRetrieveRequest();
                                ObjCustomerRetrieveRequest.ActionType = (int)Enumeration_Loyalty.ActionType.ListAll;

                                ObjCustomerRetrieveRequest.CustomerId = objQueryReplyRetrieveRequest.ActorId;
                                CustomerRetrieveResponse ObjCustomerRetrieveResponse = mobileAppBO.GetAsmSeMappingDetails(ObjCustomerRetrieveRequest);
                                if (ObjCustomerRetrieveResponse != null && ObjCustomerRetrieveResponse.lstCustomerEntityMapping.Count > 0)
                                {
                                    for (int i = 0; i < ObjCustomerRetrieveResponse.lstCustomerEntityMapping.Count; i++)
                                    {
                                        SendSMSAndEmailNotificationForMarketer(UIConstants.SMS, ObjCustomerRetrieveResponse.lstCustomerEntityMapping[i].SE_UserName, ObjCustomerRetrieveResponse.lstCustomerEntityMapping[i].SE_MobileNumber, ObjCustomerRetrieveResponse.lstCustomerEntityMapping[i].Se_FirstName, ObjCustomerRetrieveResponse.lstCustomerEntityMapping[i].CustomerUserName, args[2], Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.QueryRaiseAlertForMarketer));
                                        //SendSMSAndEmailNotificationForMarketer(UIConstants.EMAIL, ObjCustomerRetrieveResponse.lstCustomerEntityMapping[i].SE_UserName, ObjCustomerRetrieveResponse.lstCustomerEntityMapping[i].SE_EmailId, ObjCustomerRetrieveResponse.lstCustomerEntityMapping[i].Se_FirstName, ObjCustomerRetrieveResponse.lstCustomerEntityMapping[i].CustomerUserName, args[2], Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.PaymentReversalForMarketer));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "SaveCustomerQueryTicket() : " + ex.Message);
            }
            return objQueryReplyRetrieveResponse;
        }

        private void SendEmailToCustomerForPostedQuery(string TicketID, QueryReplyRetrieveRequest objQueryReplyRetrieveRequest)
        {
            try
            {
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse = new SmsEmailRetrieveResponse();

                SmsEmail ObjSmsEmail = new SmsEmail();
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.Query_ForCustomerBySelf);
                ObjSmsEmailRetrieveRequest.UserName = objQueryReplyRetrieveRequest.LoyaltyID;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = objQueryReplyRetrieveRequest.Mobile;
                ObjSmsEmailRetrieveRequest.Type = "EMAIL";
                ObjSmsEmailRetrieveResponse = mobileAppBO.GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);
                for (int i = 0; i < ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count; i++)
                {
                    List<AlertTemplateDynamicContent> DynamicTemplateEmail = new List<AlertTemplateDynamicContent>()
                {
                    new AlertTemplateDynamicContent("[CustomerName]", objQueryReplyRetrieveRequest.CustomerName),
                    new AlertTemplateDynamicContent("[TICKET_REF_NO]", TicketID),
                    new AlertTemplateDynamicContent("[HelpTopic]", objQueryReplyRetrieveRequest.HelpTopic),
                    new AlertTemplateDynamicContent("[QuerySummary]", objQueryReplyRetrieveRequest.QuerySummary),
                    new AlertTemplateDynamicContent("[QueryDetails]", objQueryReplyRetrieveRequest.QueryDetails)
                };

                    AlertUtiltityParameters alertUtiltityParameterEmail = new AlertUtiltityParameters
                                  (UIConstants.EMAIL, objQueryReplyRetrieveRequest.Email, DynamicTemplateEmail, ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i]);
                    alertUtiltityParameterEmail.ShouldUseThreading = false;
                    alertUtiltityParameterEmail.MailHeaderText = UIConstants.OTHERS_HEADER;
                    var MailResult = SendAlertUtility.SendAlert(alertUtiltityParameterEmail);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "SendEmailToCustomerForPostedQuery() : " + ex.Message);
            }
        }
        private void SendEmailToMerchantForPostedQuery(string TicketID, QueryReplyRetrieveRequest objQueryReplyRetrieveRequest)
        {
            try
            {
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse = new SmsEmailRetrieveResponse();
                SmsEmail ObjSmsEmail = new SmsEmail();
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.Query_ForMerchantFromCustomer);
                ObjSmsEmailRetrieveRequest.UserName = objQueryReplyRetrieveRequest.LoyaltyID;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = objQueryReplyRetrieveRequest.Mobile;

                ObjSmsEmailRetrieveRequest.Type = "EMAIL";
                ObjSmsEmailRetrieveResponse = mobileAppBO.GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);
                for (int i = 0; i < ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count; i++)
                {

                    List<AlertTemplateDynamicContent> DynamicTemplateEmail = new List<AlertTemplateDynamicContent>()
                {
                    new AlertTemplateDynamicContent("[Merchant]", MERCHANT_USERNAME),
                    new AlertTemplateDynamicContent("[CustomerName]", objQueryReplyRetrieveRequest.CustomerName),
                    new AlertTemplateDynamicContent("[Membership Id]", objQueryReplyRetrieveRequest.LoyaltyID),
                    new AlertTemplateDynamicContent("[TICKET_REF_NO]", TicketID),
                    new AlertTemplateDynamicContent("[HelpTopic]", objQueryReplyRetrieveRequest.HelpTopic),
                    new AlertTemplateDynamicContent("[QuerySummary]", objQueryReplyRetrieveRequest.QuerySummary),
                    new AlertTemplateDynamicContent("[QueryDetails]", objQueryReplyRetrieveRequest.QueryDetails)
                };

                    ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[0].Subject = ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[0].Subject.Replace("[TICKET_REF_NO]", TicketID);
                    AlertUtiltityParameters alertUtiltityParameterEmail = new AlertUtiltityParameters
                                  (UIConstants.EMAIL, MERCHANT_EMAIL_ID, DynamicTemplateEmail, ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i]);

                    alertUtiltityParameterEmail.ShouldUseThreading = false;
                    alertUtiltityParameterEmail.MailHeaderText = UIConstants.OTHERS_HEADER;
                    var MailResult = SendAlertUtility.SendAlert(alertUtiltityParameterEmail);

                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "SendEmailToMerchantForPostedQuery() : " + ex.Message);
            }
        }
        private void SendGetUserDetailsForPushNotification(string TicketID, QueryReplyRetrieveRequest objQueryReplyRetrieveRequest, string UserName, string Mobile, string UserType, string CustomerType = "")
        {
            try
            {
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse = new SmsEmailRetrieveResponse();
                SmsEmail ObjSmsEmail = new SmsEmail();
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                if (UserType == UIConstants.CUSTOMER)
                {
                    ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.RaiseNewQueryTicketMappedUSER);
                }
                else
                {
                    ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.RepliedYourQueryTicket);
                }
                ObjSmsEmailRetrieveRequest.UserName = UserName;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = Mobile;
                ObjSmsEmailRetrieveRequest.Type = "SMS";
                ObjSmsEmailRetrieveResponse = mobileAppBO.GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);
                for (int i = 0; i < ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count; i++)
                {
                    List<AlertTemplateDynamicContent> DynamicTemplateEmail = new List<AlertTemplateDynamicContent>()
                {
                    new AlertTemplateDynamicContent("[MEMBER]", objQueryReplyRetrieveRequest.CustomerName),
                    new AlertTemplateDynamicContent("[Query ID]", TicketID),
                    new AlertTemplateDynamicContent("[Help Topic]", objQueryReplyRetrieveRequest.HelpTopic),
                    new AlertTemplateDynamicContent("[MEMBER TYPE]", CustomerType),

                };

                    AlertUtiltityParameters alertUtiltityParameterEmail = new AlertUtiltityParameters
                                  (ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i].Type, Mobile, DynamicTemplateEmail, ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i]);

                    var MailResult = SendAlertUtility.SendAlert(alertUtiltityParameterEmail);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "SendEmailToCustomerForPostedQuery() : " + ex.Message);
            }
        }
        private void SendSMSAndEmailNotificationForMarketer(string Type, string LoyaltyId, string MobileOrEmailID, string SE_Name, string CustomerUserName, string QueryNo, string Template)
        {
            try
            {
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                ObjSmsEmailRetrieveRequest.TemplateName = Template;
                ObjSmsEmailRetrieveRequest.UserName = LoyaltyId;
                ObjSmsEmailRetrieveRequest.Type = Type;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = MobileOrEmailID;
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse_SMS = mobileAppBO.GetEmailSmsTemplate(ObjSmsEmailRetrieveRequest);

                if (ObjSmsEmailRetrieveResponse_SMS != null && ObjSmsEmailRetrieveResponse_SMS.lstSmsEmailDetails.Count > 0)
                {
                    List<AlertTemplateDynamicContent> DynamicTemplate = new List<AlertTemplateDynamicContent>()
                    {
                     new AlertTemplateDynamicContent("[User Name]", CustomerUserName),
                     new AlertTemplateDynamicContent("[Query No]",QueryNo),
                     new AlertTemplateDynamicContent("[Marketer Name]",SE_Name)
                    };

                    //Sending Email
                    if (Type == UIConstants.EMAIL)
                    {
                        for (int i = 0; i < ObjSmsEmailRetrieveResponse_SMS.lstSmsEmailDetails.Count; i++)
                        {
                            AlertUtiltityParameters alertUtiltityParametersSMS = new AlertUtiltityParameters
                                          (UIConstants.EMAIL, MobileOrEmailID, DynamicTemplate, ObjSmsEmailRetrieveResponse_SMS.lstSmsEmailDetails[i]);

                            alertUtiltityParametersSMS.VendorMethodName = SMSVendorMethodEnum.Skylinesms;
                            var SMSResult = SendAlertUtility.SendAlert(alertUtiltityParametersSMS);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < ObjSmsEmailRetrieveResponse_SMS.lstSmsEmailDetails.Count; i++)
                        {
                            AlertUtiltityParameters alertUtiltityParametersSMS = new AlertUtiltityParameters
                                      (ObjSmsEmailRetrieveResponse_SMS.lstSmsEmailDetails[i].Type, MobileOrEmailID, DynamicTemplate, ObjSmsEmailRetrieveResponse_SMS.lstSmsEmailDetails[i]);
                            // SMS will not send through thread
                            alertUtiltityParametersSMS.ShouldUseThreading = false;
                            alertUtiltityParametersSMS.VendorMethodName = SMSVendorMethodEnum.Skylinesms;
                            var SMSResult = SendAlertUtility.SendAlert(alertUtiltityParametersSMS);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "SendSMSAndEmailNotificationForMarketer() : " + ex.Message);
            }
        }
        public HelpTopicRetrieveResponse GetHelpTopics(HelpTopicRetrieveRequest objHelpTopicRetrieveRequest, string ToTranslateLanguage = "")
        {
            GoogleTranslate google = new GoogleTranslate(GOOGLE_API_KEY);
            HelpTopicRetrieveResponse objHelpTopicRetrieveResponse = new HelpTopicRetrieveResponse();
            try
            {

                objHelpTopicRetrieveResponse = mobileAppBO.GetHelpTopics(objHelpTopicRetrieveRequest);


                if (!string.IsNullOrEmpty(ToTranslateLanguage))
                {
                    objHelpTopicRetrieveResponse.objHelpTopicList = objHelpTopicRetrieveResponse.objHelpTopicList.GroupBy(x => new { x.HelpTopicId }).Select(group => group.First()).ToList();

                    List<Translation> results = google.Translate(UIConstants.DefalutLanguage, ToTranslateLanguage, (objHelpTopicRetrieveResponse.objHelpTopicList.Select(x => x.HelpTopicName).ToArray()));
                    for (int i = 0; i < objHelpTopicRetrieveResponse.objHelpTopicList.Count; i++)
                    {
                        objHelpTopicRetrieveResponse.objHelpTopicList[i].HelpTopicName = results[i].TranslatedText;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + ":: GetHelpTopics() :: " + ex.Message);

            }
            return objHelpTopicRetrieveResponse;
        }
        public QueryInfoByHelpTopicResponse GetQueryResponseInformation(QueryInfoByHelpTopicRequest objQueryInfoByHelpTopicRequest)
        {
            QueryInfoByHelpTopicResponse objQueryInfoByHelpTopicResponse = new QueryInfoByHelpTopicResponse();
            try
            {

                objQueryInfoByHelpTopicResponse = mobileAppBO.GetQueryResponseInformation(objQueryInfoByHelpTopicRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "GetQueryResponseInformation () :: " + ex.Message);
            }
            return objQueryInfoByHelpTopicResponse;
        }

        public GetScratchCodeResponse GetScratchCodeDetails(GetScratchCodeRequest ObjGetScratchCodeRequest)
        {
            GetScratchCodeResponse objGetScratchCodeResponse = null;
            try
            {
                objGetScratchCodeResponse = mobileAppBO.GetScratchCodeDetails(ObjGetScratchCodeRequest);

            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetScratchCodeDetails() " + ex.Message);
            }
            return objGetScratchCodeResponse;
        }

        public EWarrantyRetriveResponseJson GetEWarrantydetails(EWarrantyRetriveRequestJson objEWarrantyRetriveRequest)
        {
            EWarrantyRetriveResponseJson objEWarrantyRetriveResponse = null;
            try
            {
                objEWarrantyRetriveResponse = mobileAppBO.GetEWarrantydetails(objEWarrantyRetriveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetEWarrantydetails() : " + ex.Message);
            }
            return objEWarrantyRetriveResponse;
        }
        public EWarrantySaveResponse SaveEWarrantyDetails(EWarrantySaveRequest objEWarrantySaveRequest)
        {
            EWarrantySaveResponse objEWarrantySaveResponse = null;
            try
            {
                objEWarrantySaveResponse = mobileAppBO.SaveEWarrantyDetails(objEWarrantySaveRequest);
                if (objEWarrantySaveResponse.lstEWarranty != null && objEWarrantySaveResponse.lstEWarranty.Count > 0)
                {
                    if (objEWarrantySaveResponse.ReturnValue == 1)
                    {
                        EWarrantyRetriveResponse objEWarrantyRetriveResponse = new EWarrantyRetriveResponse();
                        EWarrantyRetriveRequest objEWarrantyRetriveRequest = new EWarrantyRetriveRequest();
                        objEWarrantyRetriveRequest.ActionType = 2;
                        objEWarrantyRetriveRequest.ProductId = objEWarrantySaveResponse.lstEWarranty[0].ProductId;
                        objEWarrantyRetriveRequest.EWarrantyId = objEWarrantySaveResponse.lstEWarranty[0].EWarrantyID;
                        objEWarrantyRetriveResponse = mobileAppBO.DownloadEwarranty(objEWarrantyRetriveRequest);
                        StringBuilder strbuilder = new StringBuilder();
                        strbuilder = strbuilder.Append(objEWarrantyRetriveResponse.lstEWarranty[0].ProdWarrantyDesc);
                        strbuilder.Replace("[Contractor Name]", objEWarrantyRetriveResponse.lstEWarranty[0].ContractorName);
                        strbuilder.Replace("[Customer Name]", objEWarrantyRetriveResponse.lstEWarranty[0].CustomerName);
                        strbuilder.Replace("[Customer Mobile]", objEWarrantyRetriveResponse.lstEWarranty[0].MobileNum);
                        strbuilder.Replace("[Customer Address]", objEWarrantyRetriveResponse.lstEWarranty[0].Address);
                        strbuilder.Replace("[QR Code Quantity]", objEWarrantyRetriveResponse.lstEWarranty[0].EWarrantyQty.ToString());
                        strbuilder.Replace("[Generated Date]", objEWarrantyRetriveResponse.lstEWarranty[0].EWarrantyDate);

                        SMSEmailForEWarrantyGenerateToCustomer(UIConstants.EMAIL, objEWarrantySaveRequest, objEWarrantySaveResponse.lstEWarranty[0].EWarrantyQty, objEWarrantySaveResponse.lstEWarranty[0].ProductName, objEWarrantySaveResponse.lstEWarranty[0].EWarrantyDate, strbuilder.ToString());
                        SMSEmailForEWarrantyGenerateToCustomer(UIConstants.SMS, objEWarrantySaveRequest, objEWarrantySaveResponse.lstEWarranty[0].EWarrantyQty, objEWarrantySaveResponse.lstEWarranty[0].ProductName);
                        if (objEWarrantySaveRequest.CustomerTypeId != 3)
                        {
                            SMSEmailForEWarrantyGenerateToGenerator(UIConstants.EMAIL, objEWarrantyRetriveResponse, objEWarrantySaveResponse.lstEWarranty[0].EWarrantyQty, objEWarrantySaveRequest.CustomerName, objEWarrantySaveResponse.lstEWarranty[0].ProductName, objEWarrantySaveResponse.lstEWarranty[0].EWarrantyDate, strbuilder.ToString());
                            SMSEmailForEWarrantyGenerateToGenerator(UIConstants.SMS, objEWarrantyRetriveResponse, objEWarrantySaveResponse.lstEWarranty[0].EWarrantyQty, objEWarrantySaveRequest.CustomerName, objEWarrantySaveResponse.lstEWarranty[0].ProductName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " SaveEWarrantyDetails() : " + ex.Message);
            }
            return objEWarrantySaveResponse;
        }

        private void SMSEmailForEWarrantyGenerateToCustomer(string Type, EWarrantySaveRequest objEWarrantySaveRequest, int EWarrantyQty, string ProductName = "", string EWarrantyDate = "", string ProdWarrantyDesc = "")
        {
            try
            {
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.EWarrantyGenerate);
                ObjSmsEmailRetrieveRequest.UserName = MERCHANT_USERNAME;
                if (Type == UIConstants.EMAIL)
                    ObjSmsEmailRetrieveRequest.EmailOrMobile = objEWarrantySaveRequest.EmailId;
                else
                    ObjSmsEmailRetrieveRequest.EmailOrMobile = objEWarrantySaveRequest.MobileNum;
                ObjSmsEmailRetrieveRequest.Type = Type;
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse = mobileAppBO.GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);
                for (int i = 0; i < ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count; i++)
                {
                    if (Type == UIConstants.EMAIL)
                    {
                        List<AlertTemplateDynamicContent> DynamicTemplateEmail = new List<AlertTemplateDynamicContent>()
                         {
                               new AlertTemplateDynamicContent("[Client Name]", objEWarrantySaveRequest.CustomerName),
                               new AlertTemplateDynamicContent("[Product Name]", ProductName),
                               new AlertTemplateDynamicContent("[Nos]",Convert.ToString(EWarrantyQty)),
                               new AlertTemplateDynamicContent("[E-warranty Generated Date]",EWarrantyDate),
                               new AlertTemplateDynamicContent("[table]",ProdWarrantyDesc),

                         };

                        AlertUtiltityParameters alertUtiltityParameterEmail = new AlertUtiltityParameters
                                      (UIConstants.EMAIL, objEWarrantySaveRequest.EmailId, DynamicTemplateEmail, ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i]);
                        alertUtiltityParameterEmail.ShouldUseThreading = false;
                        alertUtiltityParameterEmail.MailHeaderText = UIConstants.OTHERS_HEADER;
                        var MailResult = SendAlertUtility.SendAlert(alertUtiltityParameterEmail);
                    }
                    if (Type == UIConstants.SMS)
                    {
                        List<AlertTemplateDynamicContent> DynamicTemplateEmail = new List<AlertTemplateDynamicContent>()
                         {
                           new AlertTemplateDynamicContent("[Client Name]", objEWarrantySaveRequest.CustomerName),
                           new AlertTemplateDynamicContent("[Product Name]", ProductName),
                           new AlertTemplateDynamicContent("[Nos]",Convert.ToString(EWarrantyQty)),

                         };

                        AlertUtiltityParameters alertUtiltityParameterSMS = new AlertUtiltityParameters
                                      (UIConstants.SMS, objEWarrantySaveRequest.MobileNum, DynamicTemplateEmail, ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i]);
                        var SmSResult = SendAlertUtility.SendAlert(alertUtiltityParameterSMS);
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "SMSEmailForEWarrantyGenerateToCustomer() : " + ex.Message);
            }
        }

        private void SMSEmailForEWarrantyGenerateToGenerator(string Type, EWarrantyRetriveResponse objEWarrantyRetriveResponse, int EWarrantyQty, string CustomerName, string ProductName = "", string EWarrantyDate = "", string ProdWarrantyDesc = "")
        {
            try
            {
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.EWarrantyGeneratedBy);
                ObjSmsEmailRetrieveRequest.UserName = MERCHANT_USERNAME;
                if (Type == UIConstants.EMAIL)
                    ObjSmsEmailRetrieveRequest.EmailOrMobile = objEWarrantyRetriveResponse.lstEWarranty[0].GeneratorEmailId;
                else
                    ObjSmsEmailRetrieveRequest.EmailOrMobile = objEWarrantyRetriveResponse.lstEWarranty[0].GeneratorMobile;
                ObjSmsEmailRetrieveRequest.Type = Type;
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse = mobileAppBO.GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);
                for (int i = 0; i < ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count; i++)
                {
                    if (Type == UIConstants.EMAIL)
                    {
                        List<AlertTemplateDynamicContent> DynamicTemplateEmail = new List<AlertTemplateDynamicContent>()
                         {
                               new AlertTemplateDynamicContent("[Client Name]", objEWarrantyRetriveResponse.lstEWarranty[0].ContractorName),
                               new AlertTemplateDynamicContent("[Product Name]", ProductName),
                               new AlertTemplateDynamicContent("[Nos]",Convert.ToString(EWarrantyQty)),
                               new AlertTemplateDynamicContent("[E-warranty Generated Date]",EWarrantyDate),
                               new AlertTemplateDynamicContent("[table]",ProdWarrantyDesc),
                         };

                        AlertUtiltityParameters alertUtiltityParameterEmail = new AlertUtiltityParameters
                                      (UIConstants.EMAIL, objEWarrantyRetriveResponse.lstEWarranty[0].GeneratorEmailId, DynamicTemplateEmail, ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i]);
                        alertUtiltityParameterEmail.ShouldUseThreading = false;
                        alertUtiltityParameterEmail.MailHeaderText = UIConstants.OTHERS_HEADER;
                        var MailResult = SendAlertUtility.SendAlert(alertUtiltityParameterEmail);
                    }
                    if (ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i].Type == UIConstants.SMS)
                    {
                        List<AlertTemplateDynamicContent> DynamicTemplateEmail = new List<AlertTemplateDynamicContent>()
                         {
                           new AlertTemplateDynamicContent("[Member Name]", objEWarrantyRetriveResponse.lstEWarranty[0].ContractorName),
                           new AlertTemplateDynamicContent("[Product Name]", ProductName),
                           new AlertTemplateDynamicContent("[customer Name]",CustomerName),
                           new AlertTemplateDynamicContent("[Nos]",Convert.ToString(EWarrantyQty)),


                         };

                        AlertUtiltityParameters alertUtiltityParameterSMS = new AlertUtiltityParameters
                                      (UIConstants.SMS, objEWarrantyRetriveResponse.lstEWarranty[0].GeneratorMobile, DynamicTemplateEmail, ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i]);
                        var SmSResult = SendAlertUtility.SendAlert(alertUtiltityParameterSMS);
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "SMSEmailForEWarrantyGenerateToGenerator() : " + ex.Message);
            }
        }

        public EWarrantyRetriveResponse DownloadEwarranty(EWarrantyRetriveRequest objEWarrantyRetriveRequest)
        {
            Byte[] ConvertedPdfInByte = null;
            EWarrantyRetriveResponse objEWarrantyRetriveResponse = null;
            try
            {
                objEWarrantyRetriveResponse = mobileAppBO.DownloadEwarranty(objEWarrantyRetriveRequest);
                StringBuilder strbuilder = new StringBuilder();
                strbuilder = strbuilder.Append(objEWarrantyRetriveResponse.lstEWarranty[0].ProdWarrantyDesc);
                strbuilder.Replace("[Contractor Name]", objEWarrantyRetriveResponse.lstEWarranty[0].ContractorName);
                strbuilder.Replace("[Customer Name]", objEWarrantyRetriveResponse.lstEWarranty[0].CustomerName);
                strbuilder.Replace("[Customer Mobile]", objEWarrantyRetriveResponse.lstEWarranty[0].MobileNum);
                strbuilder.Replace("[Customer Address]", objEWarrantyRetriveResponse.lstEWarranty[0].Address);
                strbuilder.Replace("[QR Code Quantity]", objEWarrantyRetriveResponse.lstEWarranty[0].EWarrantyQty.ToString());
                strbuilder.Replace("[Generated Date]", objEWarrantyRetriveResponse.lstEWarranty[0].EWarrantyDate);
                ConvertedPdfInByte = UICommon.HtmlToPdfConvert(strbuilder);
                objEWarrantyRetriveResponse.base64Image = Convert.ToBase64String(ConvertedPdfInByte);

            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "DownloadEwarranty : " + ex.Message);
            }
            return objEWarrantyRetriveResponse;
        }

        #region QR Code Validation

        public QRCodeSaveResponse QRCodeValidation(QRCodeValidationRequest QRCodeValidationRequest)
        {
            QRCodeSaveResponse objQRCodeSaveResponse = new QRCodeSaveResponse();
            string QRCodeWithAbbrivation = QRCodeValidationRequest.QRCode.Trim();
            if (QRCodeValidationRequest != null)
            {
                try
                {
                    objQRCodeSaveResponse = new QRCodeSaveResponse();
                    objQRCodeSaveResponse = SaveQRCodeDetails(QRCodeValidationRequest);
                    if (objQRCodeSaveResponse != null && (!string.IsNullOrEmpty(objQRCodeSaveResponse.ReturnMessage)))
                    {
                        string[] arg = objQRCodeSaveResponse.ReturnMessage.Split('~');
                        QRCodeID = arg[0];
                        if (Convert.ToInt32(arg[0]) > 0)
                        {
                            string ActualQRCode = QRCodeWithAbbrivation;//.Substring(1, QRCodeWithAbbrivation.Length);
                            if (ActualQRCode.Length == 9 || ActualQRCode.Length == 12 || ActualQRCode.Length == 8)
                            {
                                string ValidateSC = ValidateScratchCode(arg[1], ActualQRCode, Convert.ToDecimal(QRCodeID));

                                if (ValidateSC.Split('~')[0].ToString() == "1")
                                {
                                    string ValidateC = ValidateCustomer(arg[1], ActualQRCode, Convert.ToDecimal(QRCodeID), QRCodeValidationRequest.DomainName);

                                    //Validate Customer
                                    if (ValidateC.Split('~')[0].ToString() == "1")
                                    {
                                        //Update Scratch Code
                                        if (!string.IsNullOrEmpty(CustomerID))
                                        {
                                            string UpdateSCD = UpdateScratchCodeDetails(ActualQRCode, CustomerID, arg[1], Convert.ToDecimal(QRCodeID));
                                            if (UpdateSCD.Split('~')[0].ToString() == "1")
                                            {
                                                //Credit Reward Points
                                                string CreditSCRP = CreditScratchCodeRewardPoints(arg[1], ActualQRCode, Convert.ToDecimal(QRCodeID));
                                                if (CreditSCRP.Split('~')[0].ToString() == "1")
                                                {
                                                    objQRCodeSaveResponse.ReturnValue = Convert.ToInt16(CreditSCRP.Split('~')[0].ToString());
                                                    objQRCodeSaveResponse.ReturnMessage = CreditSCRP.Split('~')[1].ToString();
                                                }
                                                else
                                                {
                                                    objQRCodeSaveResponse.ReturnValue = Convert.ToInt16(CreditSCRP.Split('~')[0].ToString());
                                                    objQRCodeSaveResponse.ReturnMessage = CreditSCRP.Split('~')[1].ToString();
                                                }
                                            }
                                            else if (UpdateSCD.Split('~')[0].ToString() == "3")
                                            {
                                                objQRCodeSaveResponse.ReturnValue = Convert.ToInt16(UpdateSCD.Split('~')[0].ToString());
                                                objQRCodeSaveResponse.ReturnMessage = UpdateSCD.Split('~')[1].ToString();
                                            }
                                            else
                                            {
                                                objQRCodeSaveResponse.ReturnValue = Convert.ToInt16(UpdateSCD.Split('~')[0].ToString());
                                                objQRCodeSaveResponse.ReturnMessage = UpdateSCD.Split('~')[1].ToString();
                                                UpdateTaggingStatus(Convert.ToInt32(QRCodeID), (int)Enumeration_ScratchCode.Code_Status.TechnicalIssue);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        objQRCodeSaveResponse.ReturnValue = Convert.ToInt16(ValidateC.Split('~')[0].ToString());
                                        objQRCodeSaveResponse.ReturnMessage = ValidateC.Split('~')[1].ToString();
                                        UpdateTaggingStatus(Convert.ToInt32(QRCodeID), (int)Enumeration_ScratchCode.Code_Status.InvalidMember);
                                    }
                                }
                                else
                                {
                                    objQRCodeSaveResponse.ReturnValue = Convert.ToInt16(ValidateSC.Split('~')[0].ToString());
                                    objQRCodeSaveResponse.ReturnMessage = ValidateSC.Split('~')[1].ToString();
                                    // Here no need to use UpdateTaggingStatus method, because in if block of this else condition we are
                                    // handling the status.
                                }
                            }
                            else
                            {
                                objQRCodeSaveResponse.ReturnValue = 2;
                                objQRCodeSaveResponse.ReturnMessage = "QR Code length sould not be less than or greater than 9 or 12 character. ";
                                UpdateTaggingStatus(Convert.ToInt32(QRCodeID), (int)Enumeration_ScratchCode.Code_Status.InvalidLength);
                            }
                        }
                        else
                        {
                            objQRCodeSaveResponse.ReturnValue = 2;
                            objQRCodeSaveResponse.ReturnMessage = "Your CenturyProClub account is deactivated .Please contact a CenturyPly executive or give missed call on 8955177400";
                            UpdateTaggingStatus(Convert.ToInt32(QRCodeID), (int)Enumeration_ScratchCode.Code_Status.InvalidLength);
                        }
                    }
                    else
                    {
                        objQRCodeSaveResponse.ReturnValue = 2;
                        objQRCodeSaveResponse.ReturnMessage = "QR Code details saving failed";
                        UpdateTaggingStatus(Convert.ToInt32(QRCodeID), (int)Enumeration_ScratchCode.Code_Status.TechnicalIssue);
                    }
                }
                catch (Exception ex)
                {
                    ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " :: QRCodeValidation() :: " + ex.Message);
                    objQRCodeSaveResponse.ReturnValue = -1;
                    objQRCodeSaveResponse.ReturnMessage = "Error Occoured ::" + ex.Message;
                    UpdateTaggingStatus(Convert.ToInt32(QRCodeID), (int)Enumeration_ScratchCode.Code_Status.TechnicalIssue);
                }
            }
            return objQRCodeSaveResponse;

        }

        private QRCodeSaveResponse SaveQRCodeDetails(QRCodeValidationRequest QRCodeValidationRequest)
        {
            QRCodeSaveResponse QRCodeSaveResponse = null;
            try
            {

                QRCodeSaveRequest objQRCodeSaveRequest = new QRCodeSaveRequest();

                objQRCodeSaveRequest.LoyaltyID = QRCodeValidationRequest.LoyaltyID.Trim();
                objQRCodeSaveRequest.QRCode = QRCodeValidationRequest.QRCode.Trim();
                objQRCodeSaveRequest.SourceType = QRCodeValidationRequest.SourceType;
                objQRCodeSaveRequest.ActorId = QRCodeValidationRequest.ActorId;
                objQRCodeSaveRequest.Longitude = QRCodeValidationRequest.Longitude;
                objQRCodeSaveRequest.Latitude = QRCodeValidationRequest.Latitude;

                if (QRCodeValidationRequest.objAddressInfo != null)
                {
                    objQRCodeSaveRequest.Address = QRCodeValidationRequest.objAddressInfo.AddressDetails;
                    objQRCodeSaveRequest.City = QRCodeValidationRequest.objAddressInfo.CityName;
                    objQRCodeSaveRequest.State = QRCodeValidationRequest.objAddressInfo.StateName;
                    objQRCodeSaveRequest.Country = QRCodeValidationRequest.objAddressInfo.CountryName;
                    objQRCodeSaveRequest.PinCode = QRCodeValidationRequest.objAddressInfo.Zip;
                }

                QRCodeSaveResponse = mobileAppBO.SaveQRCodeDetails(objQRCodeSaveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " :: SaveQRCodeDetails() :: " + ex.Message);
            }
            return QRCodeSaveResponse;
        }

        /// <summary>
        /// Validate Scratch Code and it will send the message to respective mobile number if any error occured during the verificeation
        /// </summary>
        /// <param name="MobileNumber"></param>
        /// <param name="ScratchCode"></param>
        /// <returns></returns>
        private string ValidateScratchCode(string MobileNumber, string ScratchCode, decimal QRCodeID)
        {
            string Result = string.Empty;
            try
            {

                ScratchCodeValidationRetrieveRequest ObjScratchCodeValidationRetrieveRequest = new ScratchCodeValidationRetrieveRequest();
                ObjScratchCodeValidationRetrieveRequest.ActionType = 2;
                ObjScratchCodeValidationRetrieveRequest.MobileNumber = MobileNumber;
                ObjScratchCodeValidationRetrieveRequest.ScratchCode = ScratchCode;
                ObjScratchCodeValidationRetrieveRequest.LongCodeID = QRCodeID;
                ObjScratchCodeValidationRetrieveRequest.CodeType = 1; //QR - CODE FROM T_LOOK_UP
                ScratchCodeValidationRetrieveResponse ObjScratchCodeValidationRetrieveResponse = mobileAppBO.ValidateScratchCode(ObjScratchCodeValidationRetrieveRequest);

                if (ObjScratchCodeValidationRetrieveResponse != null)
                {
                    if (ObjScratchCodeValidationRetrieveResponse.ValidationStatus == SCRATCH_CODE_VALID)
                    {
                        Result = "1~Success";
                    }
                    else
                    {
                        //Application Busy
                        if (ObjScratchCodeValidationRetrieveResponse.ValidationStatus == SCRATCH_CODE_BUSY)
                        {
                            string ActualResponse = SendSMS(MobileNumber, Enumeration_ScratchCode.SMS_EMail_Templates.ApplicationIsBusy, null, null, false);
                            Result = "2~" + ActualResponse;// SCRATCH_CODE_BUSY
                            UpdateTaggingStatus(Convert.ToInt32(QRCodeID), (int)Enumeration_ScratchCode.Code_Status.CodeIsBusy);
                        }
                        //Invalid Scratch Code
                        else if (ObjScratchCodeValidationRetrieveResponse.ValidationStatus == SCRATCH_CODE_INVALID)
                        //SendSMS(MobileNumber, Enumeration_ScratchCode.SMS_EMail_Templates.InvalidScratchCode);
                        {
                            string ActualResponse = SendSMS(MobileNumber, Enumeration_ScratchCode.SMS_EMail_Templates.InvalidScratchCode, null, null, false);
                            Result = "2~" + ActualResponse;// SCRATCH_CODE_INVALID
                            UpdateTaggingStatus(Convert.ToInt32(QRCodeID), (int)Enumeration_ScratchCode.Code_Status.InValidCode);
                        }
                        //Used Scratch Code
                        else if (ObjScratchCodeValidationRetrieveResponse.ValidationStatus == SCRATCH_CODE_USED)
                        //SendSMS(MobileNumber, Enumeration_ScratchCode.SMS_EMail_Templates.UsedScratchCode);
                        {
                            string ActualResponse = SendSMS(MobileNumber, Enumeration_ScratchCode.SMS_EMail_Templates.UsedScratchCode, null, null, false);
                            Result = "2~" + ActualResponse;// SCRATCH_CODE_USED
                            UpdateTaggingStatus(Convert.ToInt32(QRCodeID), (int)Enumeration_ScratchCode.Code_Status.CodeAlreadyUsed);
                        }
                        else
                        {
                            Result = "-1~Netwrork Error";
                            UpdateTaggingStatus(Convert.ToInt32(QRCodeID), (int)Enumeration_ScratchCode.Code_Status.TechnicalIssue);
                        }
                    }
                }
                else
                {
                    //Update History Table - Start
                    ScratchCodeFailureHandlingRequest ObjScratchCodeFailureHandlingRequest = new ScratchCodeFailureHandlingRequest();
                    ObjScratchCodeFailureHandlingRequest.ActionType = (int)Enumeration_ScratchCode.ScratchCode_FailureType.Scratch_Verification_Failed;
                    ObjScratchCodeFailureHandlingRequest.LongCodeID = ObjScratchCodeValidationRetrieveRequest.LongCodeID;
                    ObjScratchCodeFailureHandlingRequest.ScratchCode = ObjScratchCodeValidationRetrieveRequest.ScratchCode;
                    ObjScratchCodeFailureHandlingRequest.CodeType = 1; //QR Code Type
                    mobileAppBO.UpdateFailureDetails(ObjScratchCodeFailureHandlingRequest);
                    //Update History Table - End

                    //SendSMS(MobileNumber, Enumeration_ScratchCode.SMS_EMail_Templates.ApplicationIsBusy);
                    string ActualResponse = SendSMS(MobileNumber, Enumeration_ScratchCode.SMS_EMail_Templates.ApplicationIsBusy, null, null, false);
                    Result = "-1~" + ActualResponse;//Netwrork Error
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " :: ValidateScratchCode() :: " + ex.Message);
            }
            return Result;
        }

        /// <summary>
        /// Validate the customer is availbale or not based on the mobile number
        /// </summary>
        /// <param name="MobileNumber"></param>
        /// <param name="ScratchCode"></param>
        /// <returns></returns>
        private string ValidateCustomer(string MobileNumber, string ScratchCode, decimal QRCodeID, string DomainName)
        {
            string Result = string.Empty;
            try
            {
                //ViewState[CUSTOMER_ID] = null;
                //ViewState[CUSTOMER_NAME] = null;

                //ScratchCodeAL ObjScratchCodeAL = new ScratchCodeAL();

                ScratchCodeValidationRetrieveRequest ObjScratchCodeValidationRetrieveRequest = new ScratchCodeValidationRetrieveRequest();
                ObjScratchCodeValidationRetrieveRequest.ActionType = 2;
                ObjScratchCodeValidationRetrieveRequest.MobileNumber = MobileNumber;
                ObjScratchCodeValidationRetrieveRequest.LongCodeID = QRCodeID;
                ObjScratchCodeValidationRetrieveRequest.ScratchCode = ScratchCode;
                ObjScratchCodeValidationRetrieveRequest.CodeType = 1; //CodeType i.e. QR Code
                ObjScratchCodeValidationRetrieveRequest.DomainName = DomainName;
                ScratchCodeValidationRetrieveResponse ObjScratchCodeValidationRetrieveResponse = mobileAppBO.ValidateCustomer(ObjScratchCodeValidationRetrieveRequest);

                if (ObjScratchCodeValidationRetrieveResponse != null)
                {
                    if (ObjScratchCodeValidationRetrieveResponse.Is_Valid)
                    {
                        //ViewState[CUSTOMER_ID] = ObjScratchCodeValidationRetrieveResponse.Customer_ID;
                        //ViewState[CUSTOMER_NAME] = ObjScratchCodeValidationRetrieveResponse.Customer_Name;

                        CustomerID = ObjScratchCodeValidationRetrieveResponse.Customer_ID;
                        CustomerName = ObjScratchCodeValidationRetrieveResponse.Customer_Name;
                        CustomerEmail = ObjScratchCodeValidationRetrieveResponse.CustomerEmailID;


                        Result = "1~Success";
                    }
                    else
                    {
                        //Update History Table - Start
                        ScratchCodeFailureHandlingRequest ObjScratchCodeFailureHandlingRequest = new ScratchCodeFailureHandlingRequest();
                        ObjScratchCodeFailureHandlingRequest.ActionType = (int)Enumeration_ScratchCode.ScratchCode_FailureType.Mobile_Number_Verification_Failed;
                        ObjScratchCodeFailureHandlingRequest.LongCodeID = ObjScratchCodeValidationRetrieveRequest.LongCodeID;
                        ObjScratchCodeFailureHandlingRequest.ScratchCode = ObjScratchCodeValidationRetrieveRequest.ScratchCode;
                        ObjScratchCodeFailureHandlingRequest.CodeType = 1; //QR CODE TYPE
                        mobileAppBO.UpdateFailureDetails(ObjScratchCodeFailureHandlingRequest);

                        string ActualResponse = SendSMS(MobileNumber, Enumeration_ScratchCode.SMS_EMail_Templates.InvalidMember, null, null, false);
                        //SendSMS(MobileNumber, Enumeration_ScratchCode.SMS_EMail_Templates.InvalidMember);
                        Result = "2~" + ActualResponse;// Invalid Member
                    }
                }
                else
                {
                    //Update History Table - Start
                    ScratchCodeFailureHandlingRequest ObjScratchCodeFailureHandlingRequest = new ScratchCodeFailureHandlingRequest();
                    ObjScratchCodeFailureHandlingRequest.ActionType = (int)Enumeration_ScratchCode.ScratchCode_FailureType.Mobile_Number_Verification_Failed;
                    ObjScratchCodeFailureHandlingRequest.LongCodeID = ObjScratchCodeValidationRetrieveRequest.LongCodeID;
                    ObjScratchCodeFailureHandlingRequest.ScratchCode = ObjScratchCodeValidationRetrieveRequest.ScratchCode;
                    ObjScratchCodeFailureHandlingRequest.CodeType = 1; //QR CODE TYPE
                    mobileAppBO.UpdateFailureDetails(ObjScratchCodeFailureHandlingRequest);
                    //Update History Table - End
                    //SendSMS(MobileNumber, Enumeration_ScratchCode.SMS_EMail_Templates.ApplicationIsBusy);
                    string ActualResponse = SendSMS(MobileNumber, Enumeration_ScratchCode.SMS_EMail_Templates.ApplicationIsBusy, null, null, false);
                    Result = "-1~" + ActualResponse;//Netwrork Error
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " :: ValidateCustomer() :: " + ex.Message);
            }
            return Result;
        }

        /// <summary>
        /// Update the scratch code detials in scratch code table after the successfull customer verification
        /// </summary>
        /// <param name="ScratchCode"></param>
        /// <param name="CustomerID"></param>
        /// <param name="MobileNumber"></param>
        /// <returns>Ture : Updated Succesfull -- False: Failed </returns>
        private string UpdateScratchCodeDetails(string ScratchCode, string CustomerID, string MobileNumber, decimal QRCodeID)
        {
            string Result = string.Empty;
            try
            {


                ScratchCodeDetailsUpdateRequest ObjScratchCodeDetailsUpdateRequest = new ScratchCodeDetailsUpdateRequest();
                //ObjScratchCodeValidationRetrieveRequest.ActionType = 2;
                ObjScratchCodeDetailsUpdateRequest.ScratchCode = ScratchCode;
                ObjScratchCodeDetailsUpdateRequest.CustomerID = CustomerID;
                ObjScratchCodeDetailsUpdateRequest.LongCodeID = QRCodeID;
                ObjScratchCodeDetailsUpdateRequest.CodeType = 1; // QR Code Type
                ScratchCodeDetailsUpdateResponse ObjScratchCodeDetailsUpdateResponse = mobileAppBO.UpdateScratchCodeDetails(ObjScratchCodeDetailsUpdateRequest);
                if (ObjScratchCodeDetailsUpdateResponse != null)
                {
                    if (ObjScratchCodeDetailsUpdateResponse.UpdateStatus)
                    {
                        Result = "1~Success";
                        //SendSMS(MobileNumber, SMS_VERIFICATION_UNDER_PROCESS);
                    }
                    else
                    {
                        Result = "3~Demo QR code uploaded successfully."; // For demo QR codes
                    }
                }
                else
                {
                    //Update History Table - Start
                    ScratchCodeFailureHandlingRequest ObjScratchCodeFailureHandlingRequest = new ScratchCodeFailureHandlingRequest();
                    ObjScratchCodeFailureHandlingRequest.ActionType = (int)Enumeration_ScratchCode.ScratchCode_FailureType.Scratch_Code_Details_Updation_Failed;
                    ObjScratchCodeFailureHandlingRequest.LongCodeID = ObjScratchCodeDetailsUpdateRequest.LongCodeID;
                    ObjScratchCodeFailureHandlingRequest.ScratchCode = ObjScratchCodeDetailsUpdateRequest.ScratchCode;
                    ObjScratchCodeFailureHandlingRequest.CodeType = 1;//QR Code Type
                    mobileAppBO.UpdateFailureDetails(ObjScratchCodeFailureHandlingRequest);
                    //Update Histoty Table - End

                    string ActualResponse = SendSMS(MobileNumber, Enumeration_ScratchCode.SMS_EMail_Templates.ApplicationIsBusy, null, null, false);
                    //SendSMS(MobileNumber, Enumeration_ScratchCode.SMS_EMail_Templates.ApplicationIsBusy);
                    Result = "-1~" + ActualResponse;//Netwrork Error
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " :: UpdateScratchCodeDetails() :: " + ex.Message);
            }
            return Result;
        }

        /// <summary>
        /// Credit the points to the respective customer based on loyalty program and SKU 
        /// </summary>
        private string CreditScratchCodeRewardPoints(string MobileNumber, string ScratchCode, decimal QRCodeID)
        {
            string Result = string.Empty;
            try
            {
                //ViewState[CURRENT_BALANCE] = null;
                //ViewState[CREDITED_POINTS] = null;

                //ScratchCodeAL ObjScratchCodeAL = new ScratchCodeAL();

                CreditScratchCodeRewardPointRequest ObjCreditScratchCodeRewardPointRequest = new CreditScratchCodeRewardPointRequest();
                ObjCreditScratchCodeRewardPointRequest.ActionType = (int)Enumeration_ScratchCode.ScratchCode_ActionType.Credit_Scratch_Code_Reward_Points;
                ObjCreditScratchCodeRewardPointRequest.ScratchCode = ScratchCode;
                ObjCreditScratchCodeRewardPointRequest.LongCodeID = QRCodeID;
                ObjCreditScratchCodeRewardPointRequest.BrandName = BRAND_NAME;
                ObjCreditScratchCodeRewardPointRequest.CodeType = 1;//QR Code Type
                ObjCreditScratchCodeRewardPointRequest.DomainName = DOMAIN;
                CreditScratchCodeRewardPointResponse ObjCreditScratchCodeRewardPointResponse = mobileAppBO.CreditScratchCodeRewardPoints(ObjCreditScratchCodeRewardPointRequest);

                if (ObjCreditScratchCodeRewardPointResponse != null)
                {
                    if (ObjCreditScratchCodeRewardPointResponse.IsUpdated)
                    {
                        if (Convert.ToInt16(ObjCreditScratchCodeRewardPointResponse.CreditedPoint) == 0)
                        {
                            //SendSMS(MobileNumber, Enumeration_ScratchCode.SMS_EMail_Templates.NoLoyaltyProgram);
                            string ActualResponse = SendSMS(MobileNumber, Enumeration_ScratchCode.SMS_EMail_Templates.NoLoyaltyProgram, null, null, false);
                            Result = "2~" + ActualResponse;//NO LOYALTY PROGRAM
                            UpdateTaggingStatus(Convert.ToInt32(QRCodeID), (int)Enumeration_ScratchCode.Code_Status.SuccessButPointsNotCredited);
                        }
                        else
                        {
                            string CreditMSG = SendSMS(MobileNumber, Enumeration_ScratchCode.SMS_EMail_Templates.ValidScratchCode, ObjCreditScratchCodeRewardPointResponse.CreditedPoint, ObjCreditScratchCodeRewardPointResponse.CurrnetBalance, true);
                            Result = "1~" + CreditMSG;
                            UpdateTaggingStatus(Convert.ToInt32(QRCodeID), (int)Enumeration_ScratchCode.Code_Status.SuccessAndPointsCredited);
                            if (DOMAIN == UIConstants.AIC)
                            {
                                SendPointAccoumEmail(MobileNumber, ObjCreditScratchCodeRewardPointResponse.CreditedPoint, ObjCreditScratchCodeRewardPointResponse.CurrnetBalance);
                            }
                        }
                    }
                    else
                    {
                        //Update History Table - Start
                        ScratchCodeFailureHandlingRequest ObjScratchCodeFailureHandlingRequest = new ScratchCodeFailureHandlingRequest();
                        ObjScratchCodeFailureHandlingRequest.ActionType = (int)Enumeration_ScratchCode.ScratchCode_FailureType.Generate_Credit_Points_Failed;
                        ObjScratchCodeFailureHandlingRequest.LongCodeID = ObjCreditScratchCodeRewardPointRequest.LongCodeID;
                        ObjScratchCodeFailureHandlingRequest.ScratchCode = ObjCreditScratchCodeRewardPointRequest.ScratchCode;
                        ObjScratchCodeFailureHandlingRequest.CodeType = 1; //QRCode Type 
                        mobileAppBO.UpdateFailureDetails(ObjScratchCodeFailureHandlingRequest);
                        //Update History Table - End

                        //SendSMS(MobileNumber, Enumeration_ScratchCode.SMS_EMail_Templates.ApplicationIsBusy);
                        string ActualResponse = SendSMS(MobileNumber, Enumeration_ScratchCode.SMS_EMail_Templates.ApplicationIsBusy, null, null, false);
                        Result = "-1~" + ActualResponse;//Network Failure
                        UpdateTaggingStatus(Convert.ToInt32(QRCodeID), (int)Enumeration_ScratchCode.Code_Status.TechnicalIssue);
                    }
                }
                else
                {
                    //Update History Table - Start
                    ScratchCodeFailureHandlingRequest ObjScratchCodeFailureHandlingRequest = new ScratchCodeFailureHandlingRequest();
                    ObjScratchCodeFailureHandlingRequest.ActionType = (int)Enumeration_ScratchCode.ScratchCode_FailureType.Generate_Credit_Points_Failed;
                    ObjScratchCodeFailureHandlingRequest.LongCodeID = ObjCreditScratchCodeRewardPointRequest.LongCodeID;
                    ObjScratchCodeFailureHandlingRequest.ScratchCode = ObjCreditScratchCodeRewardPointRequest.ScratchCode;
                    ObjScratchCodeFailureHandlingRequest.CodeType = 1; //QRCode Type 
                    mobileAppBO.UpdateFailureDetails(ObjScratchCodeFailureHandlingRequest);
                    //Update History Table - End

                    string ActualResponse = SendSMS(MobileNumber, Enumeration_ScratchCode.SMS_EMail_Templates.ApplicationIsBusy, null, null, false);
                    Result = "-1~" + ActualResponse;//Network Failure
                    UpdateTaggingStatus(Convert.ToInt32(QRCodeID), (int)Enumeration_ScratchCode.Code_Status.TechnicalIssue);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " :: CreditScratchCodeRewardPoints() :: " + ex.Message);
            }
            return Result;
        }



        /// <summary>
        /// Send SMS to customer based on scratch code verification result
        /// </summary>
        /// <param name="MobileNumber"></param>
        /// <param name="SMSTemplate"></param>
        private string SendSMS(string MobileNumber, Enumeration_ScratchCode.SMS_EMail_Templates SMSTemplate, string CreditedPointBal, string CurrentPointBal, bool IsSendMessage)
        {
            StringBuilder SmsTemplate = new StringBuilder();
            string SmsTemplateResult = null;
            try
            {

                SmsEmail ObjSmsEmail = new SmsEmail();

                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(SMSTemplate);
                ObjSmsEmailRetrieveRequest.Type = UIConstants.SMS;
                ObjSmsEmailRetrieveRequest.UserName = string.Empty;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = MobileNumber;
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse = mobileAppBO.GetEmailSmsTemplate(ObjSmsEmailRetrieveRequest);
                if (ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count > 0)
                {
                    for (int i = 0; i < ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count; i++)
                    {
                        StringBuilder template = new StringBuilder();
                        template.Append(ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i].Template);

                        SmsTemplate.Append(ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i].Template);

                        List<AlertTemplateDynamicContent> DynamicTemplate = null;
                        if (SMSTemplate == Enumeration_ScratchCode.SMS_EMail_Templates.ValidScratchCode)
                        {
                            DynamicTemplate = new List<AlertTemplateDynamicContent>(){
                        new AlertTemplateDynamicContent ("[point]",CreditedPointBal),
                        new AlertTemplateDynamicContent ("[Points]",CurrentPointBal)};

                            SmsTemplate.Replace("[point]", CreditedPointBal);
                            SmsTemplate.Replace("[Points]", CurrentPointBal);
                        }
                        else
                        {
                            DynamicTemplate = new List<AlertTemplateDynamicContent>() { };
                        }
                        if (IsSendMessage)
                        {
                            AlertUtiltityParameters alertUtiltityParameters = new AlertUtiltityParameters
                            (ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i].Type, MobileNumber, DynamicTemplate, ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i]);
                            var Result = SendAlertUtility.SendAlert(alertUtiltityParameters);
                        }
                        SmsTemplateResult = SmsTemplate.ToString().Replace("T%26C", "&");
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " :: SendSMS() :: " + ex.Message);
            }
            return SmsTemplateResult;
        }


        private void SendPointAccoumEmail(string MobileNo, string PointsCredited, string CurrentPointBalance)
        {
            try
            {
                SmsEmail ObjSmsEmail = new SmsEmail();
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.PointsAccumulation);
                ObjSmsEmailRetrieveRequest.UserName = MobileNo;

                //EMAIL
                ObjSmsEmailRetrieveRequest.Type = UIConstants.EMAIL;
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse_MAIL = mobileAppBO.GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);
                for (int i = 0; i < ObjSmsEmailRetrieveResponse_MAIL.lstSmsEmailDetails.Count; i++)
                {
                    List<AlertTemplateDynamicContent> DynamicTemplate = new List<AlertTemplateDynamicContent>()
                {
                  new AlertTemplateDynamicContent("[Client name]", CustomerName),
                  new AlertTemplateDynamicContent("[Noofpoints]", PointsCredited),
                  new AlertTemplateDynamicContent("[BALANCE]", CurrentPointBalance)
                };

                    //Sending E-Mail

                    AlertUtiltityParameters alertUtiltityParametersMail = new AlertUtiltityParameters
                                  (UIConstants.EMAIL, CustomerEmail, DynamicTemplate, ObjSmsEmailRetrieveResponse_MAIL.lstSmsEmailDetails[i]);
                    alertUtiltityParametersMail.MailHeaderText = UIConstants.OTHERS_HEADER;
                    var MailResult = SendAlertUtility.SendAlert(alertUtiltityParametersMail);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " :: SendPointAccoumEmail() :: " + ex.Message);
            }

        }

        public ScratchCodeValidationRetrieveResponse ValidateScratchCode(ScratchCodeValidationRetrieveRequest ObjScratchCodeValidationRetrieveRequest)
        {
            ScratchCodeValidationRetrieveResponse objScratchCodeValidationRetrieveResponse = new ScratchCodeValidationRetrieveResponse();
            try
            {

                objScratchCodeValidationRetrieveResponse = mobileAppBO.ValidateScratchCode(ObjScratchCodeValidationRetrieveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " :: SendSMS() :: " + ex.Message);
            }
            return objScratchCodeValidationRetrieveResponse;
        }

        /// <summary>
        /// This method will update the tagging status.
        /// </summary>
        private void UpdateTaggingStatus(int TaggingID, int CodeStatus)
        {
            try
            {

                CodeStatusRetrieveRequest objCodeStatusRetrieveRequest = new CodeStatusRetrieveRequest();
                objCodeStatusRetrieveRequest.TaggingID = TaggingID;
                objCodeStatusRetrieveRequest.CodeStatus = CodeStatus;
                CodeStatusRetrieveResponse objCodeStatusRetrieveResponse = mobileAppBO.UpdateTaggingStatus(objCodeStatusRetrieveRequest);
                if (objCodeStatusRetrieveResponse != null && (!string.IsNullOrEmpty(objCodeStatusRetrieveResponse.ReturnMessage)))
                {
                    string[] args = objCodeStatusRetrieveResponse.ReturnMessage.ToString().Split('~');
                    if (args[0] == UIConstants.VALUE_MINUS_TWO)
                        ErrorHandler.WriteError(SERVICE_CLASS_NAME + "UpdateTaggingStatus ():" + args[1]);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(SERVICE_CLASS_NAME + " :: UpdateTaggingStatus() " + ex.Message);
            }
        }

        #endregion

        #region AIC QR MAPPING

        public QRCodeMappingRetrieveResponse GetAndSetQRCodeMappingInfo(QRCodeMappingRetrieveRequest objQRCodeMappingRetrieveRequest)
        {
            QRCodeMappingRetrieveResponse objQRCodeMappingRetrieveResponse = new QRCodeMappingRetrieveResponse();
            try
            {

                objQRCodeMappingRetrieveResponse = mobileAppBO.GetAndSetQRCodeMappingInfo(objQRCodeMappingRetrieveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " :: GetAndSetQRCodeMappingInfo() :: " + ex.Message);
            }
            return objQRCodeMappingRetrieveResponse;
        }

        public QRCodeMappingRetrieveResponse BulkGetAndSetQRCodeMappingInfo(QRCodeMappingRetrieveRequest objQRCodeMappingRetrieveRequest)
        {
            QRCodeMappingRetrieveResponse objQRCodeMappingRetrieveResponse = new QRCodeMappingRetrieveResponse();
            try
            {

                objQRCodeMappingRetrieveResponse = mobileAppBO.BulkGetAndSetQRCodeMappingInfo(objQRCodeMappingRetrieveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " :: BulkGetAndSetQRCodeMappingInfo() :: " + ex.Message);
            }
            return objQRCodeMappingRetrieveResponse;
        }

        #endregion AIC QR MAPPING

        #region OFFLINE_PROCESS
        public OfflineCodesResponse SaveQRCodeDetailsBulk(OfflineCodesRequest ObjOfflineCodesRequest)
        {

            OfflineCodesResponse ObjOfflineCodesResponse = new OfflineCodesResponse();
            try
            {
                ObjOfflineCodesRequest.Domain = DOMAIN;

                ObjOfflineCodesResponse = mobileAppBO.SaveQRCodeDetailsBulk(ObjOfflineCodesRequest);
                if (DOMAIN != UIConstants.CENTURY_PLY)
                {
                    if (ObjOfflineCodesResponse.QRCodeSaveResponseList != null && ObjOfflineCodesResponse.QRCodeSaveResponseList.Count > 0)
                    {
                        if (ObjOfflineCodesResponse.QRCodeSaveResponseList[0].IsNotional == 1)
                        {
                            CustomerRetrieveRequest objCustomerRetrieveRequest = new CustomerRetrieveRequest();
                            objCustomerRetrieveRequest.ActionType = 1;
                            objCustomerRetrieveRequest.LoyaltyId = ObjOfflineCodesRequest.LoyaltyID;
                            CustomerRetrieveResponse objCustomerRetrieveResponse = mobileAppBO.GetUserDetailsForPushNotification(objCustomerRetrieveRequest);
                            if (objCustomerRetrieveResponse != null && objCustomerRetrieveResponse.lstCustomer != null && objCustomerRetrieveResponse.lstCustomer.Count > 0)
                            {
                                foreach (var name in objCustomerRetrieveResponse.lstCustomer)
                                {
                                    SendGetUserDetailsForPushNotifications(name.UserName, name.Mobile, name.FirstName, name.CustomerType);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " :: SaveQRCodeDetailsBulk() :: " + ex.Message);
            }
            return ObjOfflineCodesResponse;
        }
        #endregion OFFLINE_PROCESS

        private void SendGetUserDetailsForPushNotifications(string UserName, string Mobile, string FirstName, string CustomerType)
        {
            try
            {
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse = new SmsEmailRetrieveResponse();


                SmsEmail ObjSmsEmail = new SmsEmail();
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();

                ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.CustomerMappingNotificationsAsm);

                ObjSmsEmailRetrieveRequest.UserName = UserName;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = Mobile;
                ObjSmsEmailRetrieveRequest.Type = "SMS";
                ObjSmsEmailRetrieveResponse = mobileAppBO.GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);
                for (int i = 0; i < ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count; i++)
                {
                    List<AlertTemplateDynamicContent> DynamicTemplateEmail = new List<AlertTemplateDynamicContent>()
                {
                    new AlertTemplateDynamicContent("[MEMBER]", FirstName),

                    new AlertTemplateDynamicContent("[MEMBER TYPE]", CustomerType),
                };

                    AlertUtiltityParameters alertUtiltityParameterEmail = new AlertUtiltityParameters
                                  (ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i].Type, Mobile, DynamicTemplateEmail, ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i]);

                    var MailResult = SendAlertUtility.SendAlert(alertUtiltityParameterEmail);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "SendEmailToCustomerForPostedQuery() : " + ex.Message);
            }
        }

        public SaveEventManagementDetailsResponse SaveEventManagementDetails(SaveEventManagementDetailsRequest ObjSaveEventManagementDetailsRequest)
        {
            SaveEventManagementDetailsResponse ObjSaveEventManagementDetailsResponse = null;
            try
            {
                string FullPath = string.Empty, FileName = string.Empty;
                FullPath = HTTP_UPLOAD_FOLDER_PATH + PROFILE_PIC_FOLDER_PATH;
                //Display Image
                if (ObjSaveEventManagementDetailsRequest.ObjEventImageDetails != null)
                {
                    if (ObjSaveEventManagementDetailsRequest.ObjEventImageDetails.Count > 0)
                    {
                        for (int i = 0; i < ObjSaveEventManagementDetailsRequest.ObjEventImageDetails.Count; i++)
                        {
                            FileName = "EventDoc" + DateTime.Now.ToString("ddMMyyyyhhmmss") + i + ObjSaveEventManagementDetailsRequest.ObjEventImageDetails[i].FileType;
                            bool Result = UICommon.SaveBase64StringIntoFile(FullPath.Replace("~", ""), ObjSaveEventManagementDetailsRequest.ObjEventImageDetails[i].Image, FileName);
                            ObjSaveEventManagementDetailsRequest.ObjEventImageDetails[i].Image = PROFILE_PIC_FOLDER_PATH.Replace("\\", "/") + FileName;

                        }

                    }
                }
                ObjSaveEventManagementDetailsResponse = mobileAppBO.SaveEventManagementDetails(ObjSaveEventManagementDetailsRequest);
                if (ObjSaveEventManagementDetailsResponse != null)
                {

                    if (ObjSaveEventManagementDetailsResponse.lstEventBenificiary.Count > 0)
                    {
                        foreach (var evnt in ObjSaveEventManagementDetailsResponse.lstEventBenificiary)
                        {
                            SendEventPlannerAlert(evnt.Mobile, evnt.EventAgenda, evnt.EventDate);
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " :: SaveEventManagementDetails() :: " + ex.Message);
            }
            return ObjSaveEventManagementDetailsResponse;
        }
        private void SendEventPlannerAlert(string Mobile, string Agenda, string EventDate)
        {

            string Result = string.Empty;
            try
            {
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.EventPlannerAlert);
                ObjSmsEmailRetrieveRequest.Type = UIConstants.SMS;
                ObjSmsEmailRetrieveRequest.UserName = MERCHANT_USERNAME;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = Mobile;
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponseSMS = mobileAppBO.GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);
                if (ObjSmsEmailRetrieveResponseSMS.lstSmsEmailDetails.Count > 0)
                {
                    for (int i = 0; i < ObjSmsEmailRetrieveResponseSMS.lstSmsEmailDetails.Count; i++)
                    {
                        List<AlertTemplateDynamicContent> DynamicTemplate = new List<AlertTemplateDynamicContent>() {
                      new AlertTemplateDynamicContent("[Event Date]", EventDate),
                      new AlertTemplateDynamicContent("[Agenda]", Agenda)
                     };

                        AlertUtiltityParameters alertUtiltityParameters = new AlertUtiltityParameters
                                      (ObjSmsEmailRetrieveResponseSMS.lstSmsEmailDetails[i].Type, Mobile, DynamicTemplate, ObjSmsEmailRetrieveResponseSMS.lstSmsEmailDetails[i]);
                        alertUtiltityParameters.IsGetRequest = true;
                        Result = SendAlertUtility.SendAlert(alertUtiltityParameters);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " : SendEventPlannerAlert " + ex.Message);
            }


        }

        public GetEventManagementDetailsResponse GetEventManagementMasterDetails(GetEventManagementDetailsRequest ObjGetEventManagementDetailsRequest)
        {
            GetEventManagementDetailsResponse ObjGetEventManagementDetailsResponse = null;
            try
            {
                ObjGetEventManagementDetailsResponse = mobileAppBO.GetEventManagementMasterDetails(ObjGetEventManagementDetailsRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetEventManagementMasterDetails() " + ex.Message);
            }
            return ObjGetEventManagementDetailsResponse;
        }

        public SaveTransactionResponse SaveBulkTransactionDetails(SaveTransactionRequest ObjSaveTransactionRequest)
        {
            SaveTransactionResponse ObjSaveTransactionResponse = null;
            try
            {
                ObjSaveTransactionResponse = mobileAppBO.SaveBulkTransactionDetails(ObjSaveTransactionRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " SaveBulkTransactionDetails() " + ex.Message);
            }
            return ObjSaveTransactionResponse;
        }

        public PromotionLocationDetailsRetrieveResponse GetPromotionLocationDetails(PromotionLocationDetailsRetrieveRequest ObjPromotionLocationDetailsRetrieveRequest)
        {
            PromotionLocationDetailsRetrieveResponse ObjPromotionLocationDetailsRetrieveResponse = null;
            try
            {
                ObjPromotionLocationDetailsRetrieveResponse = mobileAppBO.GetPromotionLocationDetails(ObjPromotionLocationDetailsRetrieveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetPromotionLocationDetails() " + ex.Message);
            }
            return ObjPromotionLocationDetailsRetrieveResponse;
        }

        public VehicleDetailsRetrieveResponse GetVehicleDetails(VehicleDetailsRetrieveRequest objVehicleDetailsRetrieveRequest)
        {
            VehicleDetailsRetrieveResponse ObjVehicleDetailsRetrieveResponse = null;
            try
            {
                ObjVehicleDetailsRetrieveResponse = mobileAppBO.GetVehicleDetails(objVehicleDetailsRetrieveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetVehicleDetails() " + ex.Message);
            }
            return ObjVehicleDetailsRetrieveResponse;
        }

        public LocationDetailsRetrieveResponse GetLocationDetails(LocationDetailsRetrieveRequest objLocationDetailsRetrieveRequest)
        {
            LocationDetailsRetrieveResponse ObjLocationDetailsRetrieveResponse = null;
            try
            {
                ObjLocationDetailsRetrieveResponse = mobileAppBO.GetLocationDetails(objLocationDetailsRetrieveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetVehicleDetails() " + ex.Message);
            }
            return ObjLocationDetailsRetrieveResponse;
        }

        public ProductRetriveDetailsResponse GetProductDetails(ProductRetriveRequest objProductRetriveRequest)
        {
            ProductRetriveDetailsResponse ObjProductRetriveResponse = null;
            try
            {
                ObjProductRetriveResponse = mobileAppBO.GetProductDetails(objProductRetriveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetVehicleDetails() " + ex.Message);
            }
            return ObjProductRetriveResponse;
        }

        public ReferralCustomerDetailsResponse SaveCustomerReferralDetails(ReferralCustomerDetailsRequest objReferralCustomerDetailsRequest)
        {
            ReferralCustomerDetailsResponse ObjReferralCustomerDetailsResponse = null;
            try
            {
                ObjReferralCustomerDetailsResponse = mobileAppBO.SaveCustomerReferralDetails(objReferralCustomerDetailsRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetVehicleDetails() " + ex.Message);
            }
            return ObjReferralCustomerDetailsResponse;
        }

        public CustomerSaveResponse SaveCustomerDetails(CustomerSaveRequest ObjCustomerSaveRequest)
        {
            CustomerSaveResponse objCustomerSaveResponse = null;
            try
            {
                string FullPath = string.Empty, FileName = string.Empty;
                if (ObjCustomerSaveRequest.IsMobileRequest == true)
                {
                    if (!string.IsNullOrEmpty(ObjCustomerSaveRequest.ObjCustomerJson.TradeLicence))
                    {
                        FullPath = HTTP_UPLOAD_FOLDER_PATH + "~/UploadFiles/IdentificationProof/";
                        FileName = DateTime.Now.ToString("ddMMyyyyhhmmss") + "_TradeLicence_" + ".png";
                        System.Drawing.Image CustomerImage = UICommon.Base64ToImageConverter(ObjCustomerSaveRequest.ObjCustomerJson.TradeLicence);

                        CustomerImage.Save((FullPath.Replace("~", "") + FileName), ImageFormat.Png);
                        ObjCustomerSaveRequest.ObjCustomerJson.TradeLicence = FileName;
                    }
                    if (ObjCustomerSaveRequest.ObjCustomerDetails != null)
                    {
                        if (ObjCustomerSaveRequest.ObjCustomerDetails.IsNewProfilePicture == true)
                        {
                            FullPath = HTTP_UPLOAD_FOLDER_PATH + "~/UploadFiles/CustomerImage/";
                            if (!string.IsNullOrEmpty(ObjCustomerSaveRequest.ObjCustomerDetails.ProfilePicture))
                            {
                                FileName = ObjCustomerSaveRequest.ObjCustomerDetails.LoyaltyId + DateTime.Now.ToString("ddMMyyyyhhmmss") + "_CustomerImage" + ".png";
                                if (DOMAIN == UIConstants.REDINGTON)
                                    UICommon.SaveBase64StringIntoFile(FullPath.Replace("~", ""), ObjCustomerSaveRequest.ObjCustomerDetails.ProfilePicture, FileName);
                                else
                                {
                                    System.Drawing.Image CustomerProfileImage = UICommon.Base64ToImageConverter(ObjCustomerSaveRequest.ObjCustomerDetails.ProfilePicture);
                                    CustomerProfileImage.Save((FullPath.Replace("~", "") + FileName), ImageFormat.Png);
                                }
                                ObjCustomerSaveRequest.ObjCustomerDetails.ProfilePicture = FileName;
                            }
                        }
                    }
                    if (ObjCustomerSaveRequest.ObjCustomerJson.IsUpdatePassword)
                        if (!string.IsNullOrEmpty(ObjCustomerSaveRequest.ObjCustomerJson.Password))
                        {
                            ObjCustomerSaveRequest.ObjCustomerJson.PlainPassword = ObjCustomerSaveRequest.ObjCustomerJson.Password;
                            ObjCustomerSaveRequest.ObjCustomerJson.Password = Security.EncryptPassword(ObjCustomerSaveRequest.ObjCustomerJson.Password);
                        }
                    if (DOMAIN == UIConstants.WAVIN)
                    {
                        ObjCustomerSaveRequest.ObjCustomerJson.Password = Security.EncryptPassword("654321");
                    }

                    if (!string.IsNullOrEmpty(ObjCustomerSaveRequest.ObjCustomerJson.JDOB) && (ObjCustomerSaveRequest.ObjCustomerJson.JDOB != Convert.ToString("1/1/0001")))
                        ObjCustomerSaveRequest.ObjCustomerJson.DOB = ObjCustomerSaveRequest.ObjCustomerJson.JDOB;

                    if (!string.IsNullOrEmpty(ObjCustomerSaveRequest.ObjCustomerDetails.JAnniversary) && (ObjCustomerSaveRequest.ObjCustomerDetails.JAnniversary != Convert.ToString("1/1/0001")))
                        ObjCustomerSaveRequest.ObjCustomerDetails.Anniversary = Convert.ToDateTime(ObjCustomerSaveRequest.ObjCustomerDetails.JAnniversary);
                    ObjCustomerSaveRequest.ObjCustomerJson.RELATED_PROJECT_TYPE = DOMAIN;
                    if (!string.IsNullOrEmpty(ObjCustomerSaveRequest.ObjCustomerJson.BankPassbookImage))
                    {
                        FullPath = HTTP_UPLOAD_FOLDER_PATH;
                        FileName = @"~\UploadFiles\BankImageUpload\" + ObjCustomerSaveRequest.ObjCustomerDetails.LoyaltyId + "_BankPassbookImage_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".png";
                        UICommon.SaveBase64StringIntoFile(FullPath, ObjCustomerSaveRequest.ObjCustomerJson.BankPassbookImage, FileName.Replace("~", ""));
                        ObjCustomerSaveRequest.ObjCustomerJson.BankPassbookImage = FileName.Replace('\\', '/');
                    }


                }
                objCustomerSaveResponse = mobileAppBO.SaveCustomerDetails(ObjCustomerSaveRequest);
                if (DOMAIN == UIConstants.REDINGTON || DOMAIN == UIConstants.WAVIN)
                {
                    if (objCustomerSaveResponse != null && objCustomerSaveResponse.ReturnMessage == UIConstants.VALUE_ONE)
                    {
                        if (ObjCustomerSaveRequest.ActorId == 0 && ObjCustomerSaveRequest.ObjCustomerJson.CustomerId == 0) // for Activation and Registration at both the time
                        {
                            // On Registration is same as First time Activation..
                            SendCustRegCredentials(ObjCustomerSaveRequest.ObjCustomerJson.LoyaltyId, ObjCustomerSaveRequest.ObjCustomerJson.FirstName,
                                                ObjCustomerSaveRequest.ObjCustomerJson.PlainPassword, ObjCustomerSaveRequest.ObjCustomerJson.Mobile, ObjCustomerSaveRequest.ObjCustomerJson.LoyaltyId);
                            SendCustRegCredentialsEmail(ObjCustomerSaveRequest.ObjCustomerJson.LoyaltyId, ObjCustomerSaveRequest.ObjCustomerJson.FirstName,
                                                ObjCustomerSaveRequest.ObjCustomerJson.PlainPassword, ObjCustomerSaveRequest.ObjCustomerJson.Email, ObjCustomerSaveRequest.ObjCustomerJson.LoyaltyId);
                            // On Welcome Bonus for Activation..
                            SendCustomerProfileActivationEmailAndSms(UIConstants.SMS, Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.CustomerProfileActivationBonus), ObjCustomerSaveRequest.ObjCustomerJson.CustomerType, ObjCustomerSaveRequest.ObjCustomerJson.Mobile);
                            SendCustomerProfileActivationEmailAndSms(UIConstants.EMAIL, Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.CustomerProfileActivationBonus), ObjCustomerSaveRequest.ObjCustomerJson.CustomerType, ObjCustomerSaveRequest.ObjCustomerJson.Email);
                        }
                        else // for Updating Customer Details
                        {
                            string Percentage = string.Empty; // if else for template changing on 70% and 100%
                            SendCustomerProfileUpdateEmailAndSms(UIConstants.SMS, Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.ProfileUpdate), ObjCustomerSaveRequest.ObjCustomerJson.FirstName, ObjCustomerSaveRequest.ObjCustomerJson.Mobile);
                            SendCustomerProfileUpdateEmailAndSms(UIConstants.EMAIL, Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.ProfileUpdate), ObjCustomerSaveRequest.ObjCustomerJson.FirstName, ObjCustomerSaveRequest.ObjCustomerJson.Email);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "SaveCustomerDetails : " + ex.Message);
            }
            return objCustomerSaveResponse;
        }

        private void SendCustomerProfileUpdateEmailAndSms(string Type, string TemplateName, string CustomerName, string EmailMobile)
        {
            try
            {
                string Result = string.Empty;
                SmsEmail ObjSmsEmail = new SmsEmail();
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                ObjSmsEmailRetrieveRequest.TemplateName = TemplateName;
                ObjSmsEmailRetrieveRequest.Type = Type;
                ObjSmsEmailRetrieveRequest.UserName = MERCHANT_USERNAME;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = EmailMobile;

                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse = mobileAppBO.GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);
                if (ObjSmsEmailRetrieveResponse != null && ObjSmsEmailRetrieveResponse.lstSmsEmailDetails != null && ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count > 0)
                {
                    for (int i = 0; i < ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count; i++)
                    {

                        List<AlertTemplateDynamicContent> DynamicTemplate = new List<AlertTemplateDynamicContent>()
                    {new AlertTemplateDynamicContent(ClientName, CustomerName)
                        };
                        if (Type == UIConstants.SMS)
                        {
                            AlertUtiltityParameters alertUtiltityParameters = new AlertUtiltityParameters
                                          (ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i].Type, EmailMobile, DynamicTemplate, ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i]);
                            alertUtiltityParameters.IsGetRequest = true;
                            if (DOMAIN == UIConstants.REDINGTON)
                                alertUtiltityParameters.VendorMethodName = SMSVendorMethodEnum.SmsCountry;
                            Result = SendAlertUtility.SendAlert(alertUtiltityParameters);
                        }
                        else if (Type == UIConstants.EMAIL)
                        {
                            AlertUtiltityParameters alertUtiltityParameters = new AlertUtiltityParameters
                                          (UIConstants.EMAIL, EmailMobile, DynamicTemplate, ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i]);

                            Result = SendAlertUtility.SendAlert(alertUtiltityParameters);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " SendCustomerProfileUpdateEmailAndSms() " + ex.Message);
            }
        }

        private void SendCustomerProfileActivationEmailAndSms(string Type, string TemplateName, string CustomerType, string EmailMobile)
        {
            try
            {
                string Result = string.Empty;
                SmsEmail ObjSmsEmail = new SmsEmail();
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                ObjSmsEmailRetrieveRequest.TemplateName = TemplateName;
                ObjSmsEmailRetrieveRequest.Type = Type;
                ObjSmsEmailRetrieveRequest.UserName = MERCHANT_USERNAME;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = EmailMobile;

                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse = mobileAppBO.GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);
                if (ObjSmsEmailRetrieveResponse != null && ObjSmsEmailRetrieveResponse.lstSmsEmailDetails != null && ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count > 0)
                {
                    for (int i = 0; i < ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count; i++)
                    {

                        List<AlertTemplateDynamicContent> DynamicTemplate = new List<AlertTemplateDynamicContent>()
                    {new AlertTemplateDynamicContent("[Customer Type]", CustomerType)
                        };
                        if (Type == UIConstants.SMS)
                        {
                            AlertUtiltityParameters alertUtiltityParameters = new AlertUtiltityParameters
                                          (ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i].Type, EmailMobile, DynamicTemplate, ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i]);
                            alertUtiltityParameters.IsGetRequest = true;
                            if (DOMAIN == UIConstants.REDINGTON)
                                alertUtiltityParameters.VendorMethodName = SMSVendorMethodEnum.SmsCountry;
                            Result = SendAlertUtility.SendAlert(alertUtiltityParameters);
                        }
                        else if (Type == UIConstants.EMAIL)
                        {
                            AlertUtiltityParameters alertUtiltityParameters = new AlertUtiltityParameters
                                          (UIConstants.EMAIL, EmailMobile, DynamicTemplate, ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i]);

                            Result = SendAlertUtility.SendAlert(alertUtiltityParameters);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " SendCustomerProfileActivationEmailAndSms() " + ex.Message);
            }
        }

        public CatalogueSaveResponse SaveCatalogueRedemptionDetails(CatalogueSaveRequest ObjCatalogueSaveRequest)
        {
            CatalogueSaveResponse objCatalogueSaveResponse = null;
            try
            {
                ObjCatalogueSaveRequest.Domain = DOMAIN;
                objCatalogueSaveResponse = mobileAppBO.SaveCatalogueRedemptionDetails(ObjCatalogueSaveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " SaveCatalogueRedemptionDetails() : " + ex.Message);
            }
            finally { }
            return objCatalogueSaveResponse;
        }

        public string GenerateQrCode(string LoyaltyId)
        {
            string QRCodeImageUrl = string.Empty;
            try
            {
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeGenerator.QRCode qrCode = qrGenerator.CreateQrCode(LoyaltyId, QRCodeGenerator.ECCLevel.Q);
                using (Bitmap bitMap = qrCode.GetGraphic(20))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        byte[] byteImage = ms.ToArray();
                        QRCodeImageUrl = "data:image/png;base64," + Convert.ToBase64String(byteImage);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(SERVICE_CLASS_NAME + "GenerateQrCode()" + ex.Message);
            }
            return QRCodeImageUrl;
        }

        public CustomerRetrieveResponseJson GetCustomerDetailsMobileApp(CustomerRetrieveRequestJson ObjCustomerRetrieveRequest)
        {
            CustomerRetrieveResponseJson objCustomerRetrieveResponse = null;

            try
            {
                ObjCustomerRetrieveRequest.Domain = DOMAIN;
                objCustomerRetrieveResponse = mobileAppBO.GetCustomerDetailsMobileApp(ObjCustomerRetrieveRequest);

            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "GetCustomerDetailsMobileApp : " + ex.Message);
            }
            return objCustomerRetrieveResponse;
        }

        public MerchantinfoRetriveResponse GetMerchantDetailsForCustomerEvoucher(MerchantinfoRetriveRequest ObjMerchantinfoRetriveRequest)
        {
            MerchantinfoRetriveResponse objMerchantinfoRetriveResponse = null;

            try
            {
                objMerchantinfoRetriveResponse = mobileAppBO.GetMerchantDetailsForCustomerEvoucher(ObjMerchantinfoRetriveRequest);

            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "GetCustomerDetailsMobileApp : " + ex.Message);
            }
            return objMerchantinfoRetriveResponse;
        }
        public GetTopCatalogueRetriveResponse GetTopCatalogueDetails(GetTopCatalogueRetriveRequest objGetTopCatalogueRetriveRequest)
        {
            GetTopCatalogueRetriveResponse objGetTopCatalogueRetriveResponse = null;

            try
            {
                objGetTopCatalogueRetriveResponse = mobileAppBO.GetTopCatalogueDetails(objGetTopCatalogueRetriveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "GetTopCatalogueDetails : " + ex.Message);
            }
            return objGetTopCatalogueRetriveResponse;
        }

        public UpdateCustomerCartResponse UpdateCustomerCart(UpdateCustomerCartRequest objUpdateCustomerCartRequest)
        {

            UpdateCustomerCartResponse objUpdateCustomerCartResponse = null;
            try
            {
                objUpdateCustomerCartResponse = mobileAppBO.UpdateCustomerCart(objUpdateCustomerCartRequest);
            }
            catch (Exception ex)
            {

                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " UpdateCustomerCart() : " + ex.Message);
            }
            return objUpdateCustomerCartResponse;
        }

        public BindCustomerCartRetrieveResponseApi GetCustomerCartDetails(BindCustomerCartRetrieveRequestApi objBindCustomerCartRetrieveRequest)
        {

            BindCustomerCartRetrieveResponseApi objBindCustomerCartRetrieveResponse = null;
            try
            {
                objBindCustomerCartRetrieveResponse = mobileAppBO.GetCustomerCartDetails(objBindCustomerCartRetrieveRequest);
            }
            catch (Exception ex)
            {

                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetCustomerCartDetails() : " + ex.Message);
            }
            return objBindCustomerCartRetrieveResponse;
        }
        public SaveOrderResponse SaveCustomerOrderDetails(SaveOrderRequest objSaveOrderRequest)
        {
            string FullPath, FileName = string.Empty;
            SaveOrderResponse objSaveOrderResponse = null;
            try
            {
                if (!string.IsNullOrEmpty(objSaveOrderRequest.ReceiptImage))
                {
                    FullPath = HTTP_UPLOAD_FOLDER_PATH + RECEIPT_IMAGE_FOLDER_PATH;
                    FileName = objSaveOrderRequest.LoyaltyId + "_ReceiptImage" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".png";
                    bool Result = UICommon.SaveBase64StringIntoFile(FullPath.Replace("~", ""), objSaveOrderRequest.ReceiptImage, FileName);
                    objSaveOrderRequest.ReceiptImage = FileName;
                }
                else
                {
                    objSaveOrderRequest.ReceiptImage = UIConstants.VALUE_EMPTY;
                }
                objSaveOrderResponse = mobileAppBO.SaveCustomerOrderDetails(objSaveOrderRequest);
            }
            catch (Exception ex)
            {

                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " SaveCustomerOrderDetails() : " + ex.Message);
            }
            return objSaveOrderResponse;
        }
        public WorkSiteInfoResponse GetWorkSiteDetails(WorkSiteInfoRequest objWorkSiteInfoRequest)
        {
            WorkSiteInfoResponse objWorkSiteInfoResponse = null;
            try
            {
                objWorkSiteInfoResponse = mobileAppBO.GetWorkSiteDetails(objWorkSiteInfoRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + ":: GetWorkSiteDetails ::" + ex.Message);
            }
            return objWorkSiteInfoResponse;
        }

        public DeliveryManagementDetailsResponse GetDriverDeliveryDetails(DeliveryManagementDetailsRequest objDeliveryManagementDetailsRequest)
        {
            DeliveryManagementDetailsResponse objDeliveryManagementDetailsResponse = null;
            try
            {
                objDeliveryManagementDetailsResponse = mobileAppBO.GetDriverDeliveryDetails(objDeliveryManagementDetailsRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + ":: GetDriverDeliveryDetails ::" + ex.Message);
            }
            return objDeliveryManagementDetailsResponse;
        }

        public GetOrderDetailsResponseApi GetOrderDetails(GetOrderDetailsRequest objGetOrderDetailsRequest)
        {
            GetOrderDetailsResponseApi objGetOrderDetailsResponse = null;
            try
            {
                objGetOrderDetailsResponse = mobileAppBO.GetOrderDetails(objGetOrderDetailsRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "GetUserRegionMapping()" + ex.Message);
            }
            return objGetOrderDetailsResponse;
        }

        public SaveOrderResponse SavePaymentReceiptDetails(SaveOrderRequest objSaveOrderRequest)
        {
            string FullPath, FileName = string.Empty;
            SaveOrderResponse objSaveOrderResponse = null;
            try
            {
                if (!string.IsNullOrEmpty(objSaveOrderRequest.ReceiptImage))
                {
                    FullPath = HTTP_UPLOAD_FOLDER_PATH + RECEIPT_IMAGE_FOLDER_PATH;
                    FileName = objSaveOrderRequest.LoyaltyId + "_ReceiptImage" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".png";
                    bool Result = UICommon.SaveBase64StringIntoFile(FullPath.Replace("~", ""), objSaveOrderRequest.ReceiptImage, FileName);
                    objSaveOrderRequest.ReceiptImage = FileName;
                }
                else
                    objSaveOrderRequest.ReceiptImage = UIConstants.VALUE_EMPTY;

                if (!string.IsNullOrEmpty(objSaveOrderRequest.ChequeImage))
                {
                    FullPath = HTTP_UPLOAD_FOLDER_PATH + RECEIPT_IMAGE_FOLDER_PATH;
                    FileName = objSaveOrderRequest.LoyaltyId + "_ChequeImage" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".png";
                    bool Result = UICommon.SaveBase64StringIntoFile(FullPath.Replace("~", ""), objSaveOrderRequest.ChequeImage, FileName);
                    objSaveOrderRequest.ChequeImage = FileName;
                }
                else
                    objSaveOrderRequest.ChequeImage = UIConstants.VALUE_EMPTY;

                objSaveOrderResponse = mobileAppBO.SavePaymentReceiptDetails(objSaveOrderRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "SavePaymentReceiptDetails()" + ex.Message);
            }
            return objSaveOrderResponse;
        }

        public DeliveryManagementDetailsResponse UpdateDriverDeliveryDetails(DeliveryManagementDetailsRequest objDeliveryManagementDetailsRequest)
        {
            DeliveryManagementDetailsResponse objDeliveryManagementDetailsResponse = null;
            try
            {
                objDeliveryManagementDetailsResponse = mobileAppBO.UpdateDriverDeliveryDetails(objDeliveryManagementDetailsRequest);
                if (objDeliveryManagementDetailsResponse != null)
                {
                    if (DOMAIN == UIConstants.MILLER_CLUB)
                    {
                        if (objDeliveryManagementDetailsResponse.ReturnValue == Convert.ToInt16(UIConstants.VALUE_ONE))
                        {
                            GetOrderDetailsResponseApi ObjGetOrderDetailsResponseOnOrder = null;
                            GetOrderDetailsRequest ObjGetOrderDetailsRequestOnOrder = new GetOrderDetailsRequest();
                            ObjGetOrderDetailsRequestOnOrder.ActionType = 9;
                            ObjGetOrderDetailsRequestOnOrder.OrderNumber = objDeliveryManagementDetailsRequest.OrderNo; //Mobile Number
                            ObjGetOrderDetailsResponseOnOrder = mobileAppBO.GetOrderDetails(ObjGetOrderDetailsRequestOnOrder);
                            if (ObjGetOrderDetailsResponseOnOrder != null && ObjGetOrderDetailsResponseOnOrder.lstOrderDetails.Count > 0)
                            {
                                SendCompleteDeliveryToCustomer(UIConstants.EMAIL, objDeliveryManagementDetailsRequest.CustomerLoyaltyId, objDeliveryManagementDetailsRequest.CustomerEmailId, objDeliveryManagementDetailsRequest.OrderNo, objDeliveryManagementDetailsRequest.CustomerName, objDeliveryManagementDetailsRequest.OrderAmount, ObjGetOrderDetailsResponseOnOrder.lstOrderDetails);
                                SendCompleteDeliveryToCustomer(UIConstants.SMS, objDeliveryManagementDetailsRequest.CustomerLoyaltyId, objDeliveryManagementDetailsRequest.CustomerMobileNo, objDeliveryManagementDetailsRequest.OrderNo, objDeliveryManagementDetailsRequest.CustomerName, objDeliveryManagementDetailsRequest.OrderAmount, ObjGetOrderDetailsResponseOnOrder.lstOrderDetails);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + ":: UpdateDriverDeliveryDetails ::" + ex.Message);
            }
            return objDeliveryManagementDetailsResponse;
        }

        private void SendCompleteDeliveryToCustomer(string Type, string LoyaltyId, string MobileOrEmail, string OrderNumber, string CustomerName, string OrderAmount, List<OrderProductDetailsApi> lstCart)
        {
            try
            {
                SmsEmail ObjSmsEmail = new SmsEmail();
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.OrderCompletingDelivery);
                ObjSmsEmailRetrieveRequest.Type = Type;
                ObjSmsEmailRetrieveRequest.UserName = LoyaltyId;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = MobileOrEmail;
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse = new SeMobileAppBO().GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);
                if (ObjSmsEmailRetrieveResponse != null && ObjSmsEmailRetrieveResponse.lstSmsEmailDetails != null && ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count > 0)
                {
                    if (Type == UIConstants.EMAIL)
                    {
                        // Configure Table.
                        string sbHeader = "<table border=\"1\" cellspacing=\"0\" width=\"100%\" bordercolor=\"#cccccc\" style=\"text-align: center;font-family: calibri;\" ><tr style=\"background: #e0e0e0;\"><th style=\"padding: 5px;\">ITEM</th><th style=\"padding: 5px;\">QTY</th><th style=\"padding: 5px;\">TOTAL</th></tr>";
                        string sbFooter = "</table>";

                        StringBuilder sbRowAdd = new StringBuilder();
                        StringBuilder sbFinal = new StringBuilder();
                        if (lstCart != null && lstCart.Count > 0)
                        {
                            for (int i = 0; i < lstCart[0].LstCustomerCartApi.Count; i++)
                            {
                                sbRowAdd.Append("<tr>" +
                                                   "<td style=\"padding: 5px;\">" + lstCart[0].LstCustomerCartApi[i].ProductName + "</td>" +
                                                   "<td style=\"padding: 5px;\">" + lstCart[0].LstCustomerCartApi[i].Quantity + "</td>" +
                                                   "<td style=\"padding: 5px;\">" + lstCart[0].LstCustomerCartApi[i].ProdPrice + "</td>" +
                                                "</tr>");
                            }
                            sbRowAdd.Append("<tr><td style=\"padding: 5px;\"></td><td style=\"padding: 5px;\"></td>" + Convert.ToString(lstCart[0].LstCustomerCartApi[0].NetAmount) + "</tr>");
                        }
                        sbFinal.Append(sbHeader + sbRowAdd.ToString() + sbFooter);
                        for (int i = 0; i < ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count; i++)
                        {

                            List<AlertTemplateDynamicContent> DynamicTemplate = new List<AlertTemplateDynamicContent>()
                        {
                             new AlertTemplateDynamicContent("[Client name]", CustomerName),
                             new AlertTemplateDynamicContent("[Order No]", OrderNumber),
                             new AlertTemplateDynamicContent("[Table]", sbFinal.ToString())
                        };


                            AlertUtiltityParameters alertUtiltityParameters = new AlertUtiltityParameters
                           (UIConstants.EMAIL, MobileOrEmail, DynamicTemplate, ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i]);
                            alertUtiltityParameters.VendorMethodName = SMSVendorMethodEnum.Skylinesms;
                            var Result = SendAlertUtility.SendAlert(alertUtiltityParameters);

                        }
                    }
                    else
                    {
                        List<AlertTemplateDynamicContent> DynamicTemplate = new List<AlertTemplateDynamicContent>(){
                            new AlertTemplateDynamicContent ("[Order No]", OrderNumber),
                            new AlertTemplateDynamicContent("[User Name]",LoyaltyId),
                            new AlertTemplateDynamicContent("[amount]",OrderAmount)
                            };
                        for (int i = 0; i < ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count; i++)
                        {
                            AlertUtiltityParameters alertUtiltityParameters = new AlertUtiltityParameters
                           (ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i].Type, MobileOrEmail, DynamicTemplate, ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i]);
                            alertUtiltityParameters.VendorMethodName = SMSVendorMethodEnum.Skylinesms;
                            var Result = SendAlertUtility.SendAlert(alertUtiltityParameters);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "SendCompleteDeliveryToCustomer : " + ex.Message);
            }
        }

        public WorkSiteInfoResponse SaveWorkSiteInfo(WorkSiteInfoRequest objWorkSiteInfoRequest)
        {
            WorkSiteInfoResponse objWorkSiteInfoResponse = null;
            try
            {
                objWorkSiteInfoRequest.Domain = DOMAIN;
                objWorkSiteInfoResponse = mobileAppBO.SaveWorkSiteInfo(objWorkSiteInfoRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + ":: SaveWorkSiteInfo ::" + ex.Message);
            }
            return objWorkSiteInfoResponse;
        }

        public GetScanCodeHistoryResponse GetScanCodeHistoryDetails(GetScanCodeHistoryRequest objGetScanCodeHistoryRequest)
        {
            GetScanCodeHistoryResponse objGetScanCodeHistoryResponse = null;
            try
            {
                objGetScanCodeHistoryResponse = mobileAppBO.GetScanCodeHistoryDetails(objGetScanCodeHistoryRequest);

                if (objGetScanCodeHistoryResponse != null)
                {
                    if (objGetScanCodeHistoryResponse.lstScanCodeHistory != null)
                    {
                        if (objGetScanCodeHistoryResponse.lstScanCodeHistory.Count > 0)
                        {
                            Byte[] var = GetScanCodeHistory(objGetScanCodeHistoryResponse.lstScanCodeHistory, objGetScanCodeHistoryRequest);
                            string base64String = Convert.ToBase64String(var, 0, var.Length);
                            objGetScanCodeHistoryResponse.PDF = base64String;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + ":: GetScanCodeHistoryDetails ::" + ex.Message);
            }
            return objGetScanCodeHistoryResponse;
        }

        public CountryDetailsRetrieveResponse GetCompleteCountryDetails(CountryDetailsRetrieveRequest objCountryDetailsRetrieveRequest)
        {
            CountryDetailsRetrieveResponse ObjCountryDetailsRetrieveResponse = null;
            try
            {
                ObjCountryDetailsRetrieveResponse = mobileAppBO.GetCompleteCountryDetails(objCountryDetailsRetrieveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + ":: GetScanCodeHistoryDetails ::" + ex.Message);
            }
            return ObjCountryDetailsRetrieveResponse;
        }

        public GetSOAResponse GetSOADetails(GetSOARequest objGetSOARequest)
        {
            GetSOAResponse ObjGetSOAResponse = null;
            try
            {
                ObjGetSOAResponse = mobileAppBO.GetSOADetails(objGetSOARequest);
                if (ObjGetSOAResponse != null && objGetSOARequest.ActionType == 1)
                {
                    if (ObjGetSOAResponse.LstSOA != null)
                    {
                        if (ObjGetSOAResponse.LstSOA.Count > 0)
                        {
                            Byte[] var = GetCustomerrPointsTrend(ObjGetSOAResponse.LstSOA);
                            string base64String = Convert.ToBase64String(var, 0, var.Length);
                            ObjGetSOAResponse.PDF = var;
                            ObjGetSOAResponse.PDFBase64String = base64String;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + ":: GetSOADetails ::" + ex.Message);
            }
            return ObjGetSOAResponse;
        }

        public Byte[] GetRewardTransactionPdf(List<RewardTransactionJson> LstRewardTransaction, RewardTransactionRetrieveRequest ObjGetScanCodeHistoryRequest)
        {
            Byte[] bytes = null;
            try
            {
                ReportViewer rdlcCustomerList = new ReportViewer();
                rdlcCustomerList.LocalReport.DataSources.Clear();
                rdlcCustomerList.LocalReport.ReportPath = rdlcRewardTransactionPDF;
                rdlcCustomerList.LocalReport.EnableHyperlinks = true;
                rdlcCustomerList.LocalReport.Refresh();
                ReportParameter p1 = new ReportParameter("LoyaltyId", LstRewardTransaction[0].LoyaltyId);
                ReportParameter p2 = new ReportParameter("FirstName", ObjGetScanCodeHistoryRequest.Name);
                ReportParameter p3 = new ReportParameter("FromDate", ObjGetScanCodeHistoryRequest.JFromDate);
                ReportParameter p4 = new ReportParameter("ToDate", ObjGetScanCodeHistoryRequest.JToDate);
                rdlcCustomerList.LocalReport.SetParameters(new ReportParameter[] { p1, p2, p3, p4 });
                ReportDataSource TableDataSource = new ReportDataSource(ExportTableRewardTransactionPDF, LstRewardTransaction);
                rdlcCustomerList.LocalReport.DataSources.Add(TableDataSource);
                //Export
                Warning[] warnings;
                string[] streamIds;
                string mimeType = string.Empty;
                string encoding = string.Empty;
                string extension = string.Empty;
                string fileName = "TransactionHistory" + "_" + System.DateTime.Now.ToString("ddMMyyyy_hhmm");
                bytes = rdlcCustomerList.LocalReport.Render("pdf", null, out mimeType, out encoding, out extension, out streamIds, out warnings);
                //Now that you have all the bytes representing the PDF report, buffer it and send it to the client.
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetCustomerrPointsTrend() " + ex.Message);
            }
            return bytes;
        }


        public Byte[] GetScanCodeHistory(List<ScanCodeHistory> LstScanCodeHistory, GetScanCodeHistoryRequest ObjGetScanCodeHistoryRequest)
        {
            Byte[] bytes = null;
            try
            {
                ReportViewer rdlcCustomerList = new ReportViewer();
                rdlcCustomerList.LocalReport.DataSources.Clear();

                rdlcCustomerList.LocalReport.ReportPath = rdlcReportCodeHistory;
                rdlcCustomerList.LocalReport.EnableHyperlinks = true;
                rdlcCustomerList.LocalReport.Refresh();
                ReportParameter p1 = new ReportParameter("LoyaltyId", LstScanCodeHistory[0].LoyaltyId);
                ReportParameter p2 = new ReportParameter("FirstName", LstScanCodeHistory[0].Name);
                ReportParameter p3 = new ReportParameter("FromDate", ObjGetScanCodeHistoryRequest.FromDate);
                ReportParameter p4 = new ReportParameter("ToDate", ObjGetScanCodeHistoryRequest.ToDate);
                ReportParameter p5 = new ReportParameter("TotalScans", LstScanCodeHistory[0].TotalCount.ToString());
                ReportParameter p6 = new ReportParameter("TotalSuccess", LstScanCodeHistory[0].SuccessCount.ToString());
                ReportParameter p7 = new ReportParameter("TotalPending", LstScanCodeHistory[0].PendingCount.ToString());

                rdlcCustomerList.LocalReport.SetParameters(new ReportParameter[] { p1, p2, p3, p4, p5, p6, p7 });
                ReportDataSource TableDataSource = new ReportDataSource(ExportTableCodeHistory, LstScanCodeHistory);
                rdlcCustomerList.LocalReport.DataSources.Add(TableDataSource);
                //Export
                Warning[] warnings;
                string[] streamIds;
                string mimeType = string.Empty;
                string encoding = string.Empty;
                string extension = string.Empty;
                string fileName = "ScanHistory" + "_" + System.DateTime.Now.ToString("ddMMyyyy_hhmm");
                bytes = rdlcCustomerList.LocalReport.Render("pdf", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

                //Now that you have all the bytes representing the PDF report, buffer it and send it to the client.
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetCustomerrPointsTrend() " + ex.Message);
            }
            return bytes;
        }

        public Byte[] GetCustomerrPointsTrend(List<SOA> LstSOA)
        {
            Byte[] bytes = null;
            try
            {
                ReportViewer rdlcCustomerList = new ReportViewer();
                rdlcCustomerList.LocalReport.DataSources.Clear();
                rdlcCustomerList.LocalReport.ReportPath = rdlcReportSOA;
                rdlcCustomerList.LocalReport.EnableHyperlinks = true;
                rdlcCustomerList.LocalReport.Refresh();
                ReportParameter p1 = new ReportParameter("LoyaltyId", LstSOA[0].LoyaltyId);
                ReportParameter p2 = new ReportParameter("FirstName", LstSOA[0].FirstName);
                ReportParameter p3 = new ReportParameter("MonthName", LstSOA[0].PoaGeneratedDate);
                ReportParameter p4 = new ReportParameter("NextMothDate", LstSOA[0].PointsForNextMonth);
                rdlcCustomerList.LocalReport.SetParameters(new ReportParameter[] { p1, p2, p3, p4 });
                ReportDataSource TableDataSource = new ReportDataSource(ExportTableSOA, LstSOA);
                rdlcCustomerList.LocalReport.DataSources.Add(TableDataSource);
                //Export
                Warning[] warnings;
                string[] streamIds;
                string mimeType = string.Empty;
                string encoding = string.Empty;
                string extension = string.Empty;
                string fileName = "Statement_of_Account" + "_" + System.DateTime.Now.ToString("ddMMyyyy_hhmm");
                bytes = rdlcCustomerList.LocalReport.Render("pdf", null, out mimeType, out encoding, out extension, out streamIds, out warnings);
                //Now that you have all the bytes representing the PDF report, buffer it and send it to the client.
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetCustomerrPointsTrend() " + ex.Message);
            }
            return bytes;
        }

        public EmailDomainResponse GetEmailDomain(EmailDomainRequest objEmailDomainRequest)
        {
            EmailDomainResponse objEmailDomainResponse = new EmailDomainResponse();
            try
            {
                var xmlString = File.ReadAllText(FileParameterPath);
                var stringReader = new StringReader(xmlString);
                var dsSet = new DataSet();
                dsSet.ReadXml(stringReader);

                if (objEmailDomainRequest.ActionType == 1)
                {
                    if (dsSet.Tables[1].Rows.Count > 0)
                    {
                        foreach (DataRow drow in dsSet.Tables[1].Rows)
                        {
                            if ((drow["Mobile_Text"]).ToString().ToUpper() == objEmailDomainRequest.DomainName.ToUpper())
                            {
                                objEmailDomainResponse.ReturnValue = 1;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    if (dsSet.Tables[3].Rows.Count > 0)
                    {
                        foreach (DataRow drow in dsSet.Tables[3].Rows)
                        {
                            if ((drow["EmailDomain_Text"]).ToString().ToUpper() == objEmailDomainRequest.DomainName.ToUpper())
                            {
                                objEmailDomainResponse.ReturnValue = 1;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetEmailDomain() " + ex.Message);
            }
            return objEmailDomainResponse;
        }

        public CustomerDashboardRetrieveResponse GetLedgerDashBoardDetails(CustomerDashboardRetrieveRequest ObjLedgerDashBoardDetailsRequest)
        {
            CustomerDashboardRetrieveResponse ObjLedgerDashBoardDetailsResponse = null;
            try
            {
                ObjLedgerDashBoardDetailsResponse = mobileAppBO.GetLedgerDashBoardDetails(ObjLedgerDashBoardDetailsRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "GetLedgerDashBoardDetails : " + ex.Message);
            }

            return ObjLedgerDashBoardDetailsResponse;
        }

        private void SendPendingAccountAdmin(string CustomerName, string Email, string TemplateName, string Status)
        {
            string MERCHANT_USERNAME = ConfigurationManager.AppSettings["MerchantUsername"];
            try
            {

                AttributesRetrieveRequest objAttributesRetrieveRequest = new AttributesRetrieveRequest();
                objAttributesRetrieveRequest.ActionType = Convert.ToInt32(Enumeration_Loyalty.Attribute.UserEmailId);
                objAttributesRetrieveRequest.UserType = Email;
                objAttributesRetrieveRequest.RoleIDs = UIConstants.VALUE_TWO;
                AttributesRetrieveResponse objAttributesRetrieveResponse = mobileAppBO.GetAttributeDetails(objAttributesRetrieveRequest);
                if (objAttributesRetrieveResponse != null && objAttributesRetrieveResponse.lstAttributesDetails != null && objAttributesRetrieveResponse.lstAttributesDetails.Count > 0)
                {
                    var str = String.Join(",", objAttributesRetrieveResponse.lstAttributesDetails.Select(p => p.AttributeValue));
                    SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                    ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(TemplateName);
                    ObjSmsEmailRetrieveRequest.Type = UIConstants.EMAIL;
                    ObjSmsEmailRetrieveRequest.UserName = MERCHANT_USERNAME;
                    SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse = mobileAppBO.GetEmailSmsTemplate(ObjSmsEmailRetrieveRequest);
                    for (int i = 0; i < ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count; i++)
                    {
                        List<AlertTemplateDynamicContent> DynamicTemplate = new List<AlertTemplateDynamicContent>(){
                new AlertTemplateDynamicContent ("[Date]",DateTime.Now.ToString("dd/MM/yyyy")),
                    new AlertTemplateDynamicContent ("[Email ID]",Email),
                    new AlertTemplateDynamicContent ("[Company Legal Name]",CustomerName),
                        new AlertTemplateDynamicContent ("[Account Status]",Status)};

                        AlertUtiltityParameters alertUtiltityParameters = new AlertUtiltityParameters
                       (UIConstants.EMAIL, str, DynamicTemplate, ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i]);
                        alertUtiltityParameters.IsGetRequest = true;

                        var Result = SendAlertUtility.SendAlert(alertUtiltityParameters);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " SendPendingAccountAdmin() : " + ex.Message);
            }
        }

        public RedemptionPlannerRetriveResponseJson GetPlannerAddedOrNot(RedemptionPlannerRetriveRequestJson objPlannerRetriveRequest)
        {
            RedemptionPlannerRetriveResponseJson objRedemptionPlannerRetriveResponse = null;
            try
            {
                objRedemptionPlannerRetriveResponse = mobileAppBO.GetPlannerAddedOrNot(objPlannerRetriveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetPlannerAddedOrNot() " + ex.Message);
            }
            return objRedemptionPlannerRetriveResponse;
        }

        public ProductRetriveResponseAPI BindProductDetails(ProductRetriveRequest objProductRetriveRequest)
        {
            ProductRetriveResponseAPI objProductRetriveResponseAPI = null;
            try
            {
                objProductRetriveResponseAPI = mobileAppBO.BindProductDetails(objProductRetriveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetPlannerAddedOrNot() " + ex.Message);
            }
            return objProductRetriveResponseAPI;
        }

        public RedemptionPlannerRetriveResponseJson GetRedemptionPlannerDetails(RedemptionPlannerRetriveRequestJson objPlannerRetriveRequest)
        {
            RedemptionPlannerRetriveResponseJson objRedemptionPlannerRetriveResponse = null;
            try
            {
                objPlannerRetriveRequest.Domain = DOMAIN;
                objRedemptionPlannerRetriveResponse = mobileAppBO.GetRedemptionPlannerDetails(objPlannerRetriveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetPlannerAddedOrNot() " + ex.Message);
            }
            return objRedemptionPlannerRetriveResponse;
        }
        public ProductSaveResponse SaveEGiftRequestMyEssilorRewards(ProductSaveRequest objProductSaveRequest)
        {
            ProductSaveResponse objProductSaveResponse = null;
            try
            {
                string FullPath = string.Empty, FileName = string.Empty;
                FullPath = HTTP_UPLOAD_FOLDER_PATH + PROFILE_PIC_FOLDER_PATH;
                //Display Image
                if (!string.IsNullOrEmpty(objProductSaveRequest.ProductSaveDetails.ProductImage))
                {
                    FileName = objProductSaveRequest.LoyaltyID + "_Claim_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".png";
                    System.Drawing.Image CustomerImage = UICommon.Base64ToImageConverter(objProductSaveRequest.ProductSaveDetails.ProductImage);

                    CustomerImage.Save((FullPath.Replace("~", "") + FileName), ImageFormat.Png);
                    objProductSaveRequest.ProductSaveDetails.ProductImage = PROFILE_PIC_FOLDER_PATH.Replace("\\", "/") + FileName;
                }
                objProductSaveResponse = mobileAppBO.SaveEGiftRequestMyEssilorRewards(objProductSaveRequest);

            }
            catch (Exception ex)
            {

                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " : SaveEGiftRequestMyEssilorRewards() : " + ex.Message);
            }

            return objProductSaveResponse;
        }

        public LoyaltyProgRetrieveResponseJson GetLoyaltyProgramsForERequestJson(LoyaltyProgRetrieveRequestJson objLoyaltyProgRetrieveRequest)
        {
            LoyaltyProgRetrieveResponseJson objLoyaltyProgRetrieveResponse = null;
            try
            {
                objLoyaltyProgRetrieveRequest.Domain = DOMAIN;
                objLoyaltyProgRetrieveResponse = mobileAppBO.GetLoyaltyProgramsForERequestJson(objLoyaltyProgRetrieveRequest);
            }
            catch (Exception ex)
            {

                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " : GetLoyaltyProgramsForERequestJson : " + ex.Message);
            }
            return objLoyaltyProgRetrieveResponse;
        }
        public CustomerClaimRetrieveResponseJson BindAssessmentRequestDetails(CustomerClaimRetrieveRequestJson ObjCustomerRetrieveRequest)
        {
            CustomerClaimRetrieveResponseJson objCustomerClaimRetrieveResponseJson = null;
            try
            {
                objCustomerClaimRetrieveResponseJson = mobileAppBO.BindAssessmentRequestDetails(ObjCustomerRetrieveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + ":: BindAssessmentRequestDetails ::" + ex.Message);
            }
            return objCustomerClaimRetrieveResponseJson;
        }
        public GiftCardIssueSaveResponseJson SaveGiftCardIssueDetails(GiftCardIssueSaveRequestJson ObjGiftCardIssueSaveRequest)
        {
            GiftCardIssueSaveResponseJson objGiftCardIssueSaveResponse = null;
            try
            {

                decimal Balance = 0;
                string Name = string.Empty, Email = string.Empty,
                         Mobile = string.Empty, SelectedGiftCardType = string.Empty,
                         IssuedCardNumber = string.Empty, CuDetails = string.Empty, CardNo = string.Empty,
                         Exp_Date = string.Empty, FullName = string.Empty, CardPinType = string.Empty;

                GiftCardIssueRetrieveResponse ObjGiftCardIssueRetrieveResponse = new GiftCardIssueRetrieveResponse();
                GiftCardIssueRetrieveRequest ObjGiftCardIssueRetrieveRequest = new GiftCardIssueRetrieveRequest();
                ObjGiftCardIssueRetrieveRequest.ActionType = (int)Enumeration_Loyalty.ActionType.GetMerchantDetails_For_Message;
                ObjGiftCardIssueRetrieveRequest.ActorId = ObjGiftCardIssueSaveRequest.ActorId;
                ObjGiftCardIssueRetrieveRequest.LoyaltyId = ObjGiftCardIssueSaveRequest.GiftCardIssueDetails.GiftCardCatName;
                ObjGiftCardIssueRetrieveRequest.CardTypeId = ObjGiftCardIssueSaveRequest.GiftCardIssueDetails.GiftCardTypeId;
                ObjGiftCardIssueRetrieveResponse = getGiftCardIssueMobileApp(ObjGiftCardIssueRetrieveRequest);
                if (!string.IsNullOrEmpty(ObjGiftCardIssueRetrieveResponse.ReturnMessage))
                    ObjGiftCardIssueSaveRequest.GiftCardIssueDetails.CardNumber = ObjGiftCardIssueRetrieveResponse.ReturnMessage;


                ObjGiftCardIssueRetrieveRequest.ActionType = (int)Enumeration_Loyalty.ActionType.Get_Gift_Card_Pin_Type;
                ObjGiftCardIssueRetrieveRequest.ActorId = ObjGiftCardIssueSaveRequest.ActorId;
                ObjGiftCardIssueRetrieveRequest.LoyaltyId = ObjGiftCardIssueSaveRequest.GiftCardIssueDetails.GiftCardCatName;
                ObjGiftCardIssueRetrieveResponse = getGiftCardIssueMobileApp(ObjGiftCardIssueRetrieveRequest);
                if (ObjGiftCardIssueRetrieveResponse.ReturnMessage != null)
                    CardPinType = ObjGiftCardIssueRetrieveResponse.ReturnMessage;


                ObjGiftCardIssueRetrieveRequest.ActionType = (int)Enumeration_Loyalty.ActionType.Get_Customer_Details_BY_LoyaltyID;
                ObjGiftCardIssueRetrieveRequest.ActorId = ObjGiftCardIssueSaveRequest.ActorId;
                ObjGiftCardIssueRetrieveRequest.LoyaltyId = ObjGiftCardIssueSaveRequest.GiftCardIssueDetails.LoyaltyId;
                ObjGiftCardIssueRetrieveResponse = getGiftCardIssueMobileApp(ObjGiftCardIssueRetrieveRequest);
                CuDetails = ObjGiftCardIssueRetrieveResponse.ReturnMessage;

                string[] CustomerDetails = CuDetails.Split('~');
                if (CustomerDetails.Length > 1)
                {
                    Name = CustomerDetails[0];
                    Email = CustomerDetails[1];
                    Mobile = CustomerDetails[2];
                    FullName = CustomerDetails[3];
                    SelectedGiftCardType = ObjGiftCardIssueSaveRequest.GiftCardIssueDetails.GiftCardCatName;
                    IssuedCardNumber = ObjGiftCardIssueSaveRequest.GiftCardIssueDetails.CardNumber;
                }
                string Password = UICommon.GetRandomNumber();
                ObjGiftCardIssueSaveRequest.GiftCardIssueDetails.Password = UICommon.Encrypt(Password);

                objGiftCardIssueSaveResponse = mobileAppBO.SaveGiftCardIssueDetails(ObjGiftCardIssueSaveRequest);
                if (objGiftCardIssueSaveResponse != null)
                {
                    string[] arg = null;
                    int returnValue = 0;
                    if (objGiftCardIssueSaveResponse.ReturnMessage.ToString().Contains(":"))
                    {
                        arg = objGiftCardIssueSaveResponse.ReturnMessage.ToString().Split(':');
                        returnValue = Convert.ToInt32(arg[1]);
                    }
                    else
                    {
                        returnValue = Convert.ToInt32(objGiftCardIssueSaveResponse.ReturnMessage);
                    }
                    if (returnValue > 0)
                    {
                        List<MerchantInfo> lstMerchantinfo = new List<MerchantInfo>();
                        MerchantinfoRetriveResponse ObjMerchantinfoRetriveResponse = new MerchantinfoRetriveResponse();
                        MerchantinfoRetriveRequest ObjMerchantinfoRetriveRequest = new MerchantinfoRetriveRequest();
                        ObjMerchantinfoRetriveRequest.ActionType = (int)Enumeration_Loyalty.ActionType.Get_Evoucher_Details;
                        ObjMerchantinfoRetriveRequest.ActorId = ObjGiftCardIssueSaveRequest.ActorId;
                        ObjMerchantinfoRetriveResponse = GetMerchantDetailsForCustomerEvoucher(ObjMerchantinfoRetriveRequest);
                        lstMerchantinfo = ObjMerchantinfoRetriveResponse.lstMerchantinfo.FindAll(x => x.MerchantID == returnValue).ToList();
                        string[] CardDetails = lstMerchantinfo[0].MerchantName.ToString().Split('~');
                        Balance = Convert.ToDecimal(CardDetails[3]);
                        Exp_Date = CardDetails[6];
                        sendMail(FullName, Email, Password, SelectedGiftCardType, IssuedCardNumber, CardPinType, Balance, Exp_Date);
                        sendPasswordSms(Name, Mobile, Password, SelectedGiftCardType, IssuedCardNumber, CardPinType, Balance, Exp_Date);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "SaveGiftCardIssueDetails : " + ex.Message);
            }

            return objGiftCardIssueSaveResponse;
        }
        private void sendMail(string CustomerName, string Email, string password, string GiftCardType, string CardNumber, string CardPinType, Decimal Balance, string Exp_Date)
        {
            try
            {
                string CUSTOMER_NAME = "[client name]",
                  GIFT_CARD_NAME = "[Gift card name]",
                  CARD_NUMBER = "[card #]",
                  CARD_BALANCE = "[Card Value]",
                  CARD_EXPIRY_DATE = "[Expiry Date]";
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                if (CardPinType == UIConstants.SAVED_PIN)
                {
                    ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.IssueOfRewardCardWithPIN);
                }
                else
                {
                    ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.IssueOfRewardCardWithOutPINWithOTP);
                }
                ObjSmsEmailRetrieveRequest.Type = UIConstants.EMAIL;
                ObjSmsEmailRetrieveRequest.UserName = MERCHANT_USERNAME;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = Email;

                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse = mobileAppBO.GetEmailSmsTemplate(ObjSmsEmailRetrieveRequest);
                if (ObjSmsEmailRetrieveResponse != null && ObjSmsEmailRetrieveResponse.lstSmsEmailDetails != null && ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count > 0)
                {
                    for (int i = 0; i < ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count; i++)
                    {
                        //Dynamic template for SMS
                        List<AlertTemplateDynamicContent> DynamicTemplateSMS = new List<AlertTemplateDynamicContent>()
                        {
                                new AlertTemplateDynamicContent(PIN, password),
                                new AlertTemplateDynamicContent(CUSTOMER_NAME, CustomerName),
                                new AlertTemplateDynamicContent(CARD_NUMBER, CardNumber),
                                new AlertTemplateDynamicContent(CARD_BALANCE, Convert.ToString(Balance)),
                                new AlertTemplateDynamicContent(CARD_EXPIRY_DATE, Exp_Date),
                                new AlertTemplateDynamicContent(GIFT_CARD_NAME, GiftCardType)
                        };

                        //For SMS
                        AlertUtiltityParameters alertUtiltityParametersSMS = new AlertUtiltityParameters
                                      (UIConstants.EMAIL, Email, DynamicTemplateSMS, ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i]);

                        alertUtiltityParametersSMS.IsGetRequest = true;
                        var SMSResult = SendAlertUtility.SendAlert(alertUtiltityParametersSMS);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "SaveGiftCardIssueDetails : " + ex.Message);
            }
        }
        private void sendPasswordSms(string CustomerName, string Mobile, string password, string GiftCardType, string CardNumber, string CardPinType, decimal Balance, string Exp_Date)
        {
            try
            {
                string CUSTOMER_NAME = "[client name]",
                 GIFT_CARD_NAME = "[Gift card name]",
                 CARD_NUMBER = "[card #]",
                 CARD_BALANCE = "[Card Value]",
                 CARD_EXPIRY_DATE = "[Expiry Date]";
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                if (CardPinType == UIConstants.SAVED_PIN)
                {
                    ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.IssueOfRewardCardWithPIN);
                }
                else
                {
                    ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.IssueOfRewardCardWithOutPINWithOTP);
                }
                ObjSmsEmailRetrieveRequest.Type = UIConstants.SMS;
                ObjSmsEmailRetrieveRequest.UserName = MERCHANT_USERNAME;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = Mobile;

                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse = mobileAppBO.GetEmailSmsTemplate(ObjSmsEmailRetrieveRequest);
                if (ObjSmsEmailRetrieveResponse != null && ObjSmsEmailRetrieveResponse.lstSmsEmailDetails != null && ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count > 0)
                {
                    for (int i = 0; i < ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count; i++)
                    {
                        //Dynamic template for SMS
                        List<AlertTemplateDynamicContent> DynamicTemplateSMS = new List<AlertTemplateDynamicContent>()
                        {
                                new AlertTemplateDynamicContent(PIN, password),
                                new AlertTemplateDynamicContent(CUSTOMER_NAME, CustomerName),
                                new AlertTemplateDynamicContent(CARD_NUMBER, CardNumber),
                                new AlertTemplateDynamicContent(CARD_BALANCE, Convert.ToString(Balance)),
                                new AlertTemplateDynamicContent(CARD_EXPIRY_DATE, Exp_Date),
                                new AlertTemplateDynamicContent(GIFT_CARD_NAME, GiftCardType)
                        };
                        //For SMS
                        AlertUtiltityParameters alertUtiltityParametersSMS = new AlertUtiltityParameters
                                      (ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i].Type, Mobile, DynamicTemplateSMS, ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i]);

                        alertUtiltityParametersSMS.IsGetRequest = true;
                        var SMSResult = SendAlertUtility.SendAlert(alertUtiltityParametersSMS);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "SaveGiftCardIssueDetails : " + ex.Message);
            }
        }

        public CashBackRetrieveResponse GetCashBackDetails(CashBackRetrieveRequest objCashBackRetrieveRequest)
        {
            CashBackRetrieveResponse objCashBackRetrieveResponse = null;
            try
            {
                objCashBackRetrieveResponse = mobileAppBO.GetCashBackDetails(objCashBackRetrieveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "GetCashBackDetails() : " + ex.Message);
            }
            return objCashBackRetrieveResponse;
        }
        public PromotionDetailsSaveResponseJson GetAlbumsWithImages(PromotionDetailsSaveRequestJson ObjPromotionDetailsSaveRequest)
        {
            PromotionDetailsSaveResponseJson ObjPromotionDetailsSaveResponse = null;
            try
            {
                ObjPromotionDetailsSaveResponse = mobileAppBO.GetAlbumsWithImages(ObjPromotionDetailsSaveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "GetAlbumsWithImages : " + ex.Message);
            }
            finally
            {
            }
            return ObjPromotionDetailsSaveResponse;
        }

        public PromotionDetailsSaveResponseJson GetAlbumImagesById(PromotionDetailsSaveRequestJson ObjPromotionDetailsSaveRequest)
        {
            PromotionDetailsSaveResponseJson ObjPromotionDetailsSaveResponse = null;
            try
            {
                ObjPromotionDetailsSaveResponse = mobileAppBO.GetAlbumImagesById(ObjPromotionDetailsSaveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "GetAlbumImagesById : " + ex.Message);
            }
            finally
            {
            }
            return ObjPromotionDetailsSaveResponse;
        }

        public CityRetrieveResponse GetCitiesBasedOnCountry(CityRetrieveRequest ObjCityRetrieveRequest)
        {
            CityRetrieveResponse objCityRetrieveResponse = null;
            try
            {
                objCityRetrieveResponse = mobileAppBO.GetCitiesBasedOnCountry(ObjCityRetrieveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " :: GetCitiesBasedOnCountry() : " + ex.Message);
            }
            return objCityRetrieveResponse;
        }

        public CustomerFeedBackResponse SaveCustomerTicketFeedback(SaveCustomerFeedBackRequest ObjSaveCustomerFeedBackRequest)
        {
            CustomerFeedBackResponse objCustomerFeedBackResponse = null;
            try
            {
                objCustomerFeedBackResponse = mobileAppBO.SaveCustomerTicketFeedback(ObjSaveCustomerFeedBackRequest);
            }
            catch (Exception ex)
            {

                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " :: SaveCustomerTicketFeedback() : " + ex.Message);
            }
            return objCustomerFeedBackResponse;
        }

        public CustomerDashboardRetrieveResponse GetBehaviourWiseEarning(CustomerDashboardRetrieveRequest ObjCustomerDashboardRetrieveReq)
        {
            CustomerDashboardRetrieveResponse objCustomerDashboardRetrieveResponse = null;
            try
            {
                objCustomerDashboardRetrieveResponse = mobileAppBO.GetBehaviourWiseEarning(ObjCustomerDashboardRetrieveReq);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "GetBehaviourWiseEarning : " + ex.Message);
            }
            return objCustomerDashboardRetrieveResponse;
        }

        public DreamGiftRetriveResponseJson GetDreamGiftDetails(DreamGiftRetriveRequestJson objDreamGiftRetriveRequest)
        {
            DreamGiftRetriveResponseJson ObjDreamGiftRetriveResponse = null;
            try
            {
                objDreamGiftRetriveRequest.Domain = DOMAIN;
                ObjDreamGiftRetriveResponse = mobileAppBO.GetDreamGiftDetails(objDreamGiftRetriveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetDreamGiftDetails : " + ex.Message);
            }
            return ObjDreamGiftRetriveResponse;
        }

        public DreamGiftSaveResponse SaveOrUpdateDreamGiftDetails(DreamGiftSaveRequest objDreamGiftSaveRequest)
        {
            DreamGiftSaveResponse ObjDreamGiftSaveResponse = null;
            try
            {
                ObjDreamGiftSaveResponse = mobileAppBO.SaveOrUpdateDreamGiftDetails(objDreamGiftSaveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " SaveOrUpdateDreamGiftDetails : " + ex.Message);
            }
            return ObjDreamGiftSaveResponse;
        }
        public ReferralDetailsResponse GetReferralDetails(ReferralDetailsRequest objReferralDetailsRequest)
        {
            ReferralDetailsResponse objReferralDetailsResponse = null;
            try
            {
                objReferralDetailsResponse = mobileAppBO.GetReferralDetails(objReferralDetailsRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "GetReferralDetails: " + ex.Message);
            }
            return objReferralDetailsResponse;

        }

        public InsuranceFileUploadRetrieveResponse GetInsuranceDetails(InsuranceFileUploadRetrieveRequest objInsuranceFileUploadRetrieveRequest)
        {
            InsuranceFileUploadRetrieveResponse objInsuranceFileUploadRetrieveResponse = null;
            try
            {
                objInsuranceFileUploadRetrieveResponse = mobileAppBO.GetInsuranceDetails(objInsuranceFileUploadRetrieveRequest);

                if (objInsuranceFileUploadRetrieveResponse != null && objInsuranceFileUploadRetrieveResponse.UploadResultDetailsList.Count > 0)
                {
                    StringBuilder strbuilder = new StringBuilder();
                    strbuilder = strbuilder.Append(objInsuranceFileUploadRetrieveResponse.UploadResultDetailsList[0].Description);
                    strbuilder.Replace("[MemberName]", objInsuranceFileUploadRetrieveResponse.UploadResultDetailsList[0].MemberName);
                    strbuilder.Replace("[MemberDob]", objInsuranceFileUploadRetrieveResponse.UploadResultDetailsList[0].MemberDob);
                    strbuilder.Replace("[NomineeName]", objInsuranceFileUploadRetrieveResponse.UploadResultDetailsList[0].NomineeName);
                    strbuilder.Replace("[NomineeMobile]", objInsuranceFileUploadRetrieveResponse.UploadResultDetailsList[0].Mobile);
                    strbuilder.Replace("[NomineeRelation]", objInsuranceFileUploadRetrieveResponse.UploadResultDetailsList[0].NomineeRelation);
                    strbuilder.Replace("[InsuranceType]", objInsuranceFileUploadRetrieveResponse.UploadResultDetailsList[0].InsuranceType);
                    strbuilder.Replace("[InsuranceID]", objInsuranceFileUploadRetrieveResponse.UploadResultDetailsList[0].InsuranceID);
                    strbuilder.Replace("[StartDate]", objInsuranceFileUploadRetrieveResponse.UploadResultDetailsList[0].StartDate);
                    strbuilder.Replace("[ExpiryDate]", objInsuranceFileUploadRetrieveResponse.UploadResultDetailsList[0].ExpiryDate + " till midnight");
                    strbuilder.Replace("[InsuranceAmount]", objInsuranceFileUploadRetrieveResponse.UploadResultDetailsList[0].InsuranceAmount);
                    objInsuranceFileUploadRetrieveResponse.UploadResultDetailsList[0].Certificate = UICommon.HtmlToPdfConvert(strbuilder);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "GetInsuranceDetails : " + ex.Message);
                return objInsuranceFileUploadRetrieveResponse;
            }
            return objInsuranceFileUploadRetrieveResponse;
        }


        public GamificationTransactionResponse GetGamificationTransaction(GamificationTransactionRequest objGamificationTransactionRequest)
        {
            GamificationTransactionResponse objGamificationTransactionResponse = null;
            try
            {
                objGamificationTransactionResponse = mobileAppBO.GetGamificationTransaction(objGamificationTransactionRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "GetGamificationTransaction : " + ex.Message);
            }
            return objGamificationTransactionResponse;

        }

        public GamificationTransactionResponse UpdateGamificationTransaction(GamificationTransactionRequest objGamificationTransactionRequest)
        {
            GamificationTransactionResponse objGamificationTransactionResponse = null;
            try
            {
                objGamificationTransactionResponse = mobileAppBO.UpdateGamificationTransaction(objGamificationTransactionRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "UpdateGamificationTransaction : " + ex.Message);
            }
            return objGamificationTransactionResponse;

        }

        public GetOrderDetailsResponseApi ApproveOrderDetails(GetOrderDetailsRequestApi objGetOrderDetailsRequest)
        {
            GetOrderDetailsResponseApi objGetOrderDetailsResponse = null;
            try
            {
                objGetOrderDetailsResponse = mobileAppBO.ApproveOrderDetails(objGetOrderDetailsRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "ApproveOrderDetails()" + ex.Message);
            }
            return objGetOrderDetailsResponse;
        }

        private void SendWelcomeAccountActivationSMS(string CustomerName, string Mobile)
        {
            try
            {
                string Result = string.Empty;
                SmsEmail ObjSmsEmail = new SmsEmail();
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.WelcomeAccountactivation);
                ObjSmsEmailRetrieveRequest.Type = UIConstants.SMS;
                ObjSmsEmailRetrieveRequest.UserName = MERCHANT_USERNAME;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = Mobile;

                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse = mobileAppBO.GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);
                if (ObjSmsEmailRetrieveResponse != null && ObjSmsEmailRetrieveResponse.lstSmsEmailDetails != null && ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count > 0)
                {
                    for (int i = 0; i < ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count; i++)
                    {

                        List<AlertTemplateDynamicContent> DynamicTemplate = new List<AlertTemplateDynamicContent>()
                    {new AlertTemplateDynamicContent(ClientName, CustomerName)
                        };

                        AlertUtiltityParameters alertUtiltityParameters = new AlertUtiltityParameters
                                      (ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i].Type, Mobile, DynamicTemplate, ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i]);
                        alertUtiltityParameters.IsGetRequest = true;
                        if (DOMAIN == UIConstants.MILLER_CLUB)
                            alertUtiltityParameters.VendorMethodName = SMSVendorMethodEnum.Skylinesms;
                        Result = SendAlertUtility.SendAlert(alertUtiltityParameters);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " SendWelcomeAccountActivationSMS() " + ex.Message);
            }
        }

        public CustomerDashboardRetrieveResponse GetDashBoardDetailsApi(CustomerDashboardRetrieveRequest objCustomerDashboardRetrieveRequest)
        {
            CustomerDashboardRetrieveResponse objCustomerDashboardRetrieveResponse = null;
            try
            {
                objCustomerDashboardRetrieveResponse = mobileAppBO.GetDashBoardDetailsApi(objCustomerDashboardRetrieveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "GetDashBoardDetailsApi()" + ex.Message);
            }
            return objCustomerDashboardRetrieveResponse;
        }

        public MerchantDetailsJsonRetriveResponse GetMerchantListJson(MerchantDetailsJsonRetriveRequest objMerchantDetailsJsonRetriveRequest)
        {
            MerchantDetailsJsonRetriveResponse objMerchantDetailsJsonRetriveResponse = null;
            try
            {
                objMerchantDetailsJsonRetriveResponse = mobileAppBO.GetMerchantListJson(objMerchantDetailsJsonRetriveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "GetMerchantListJson()" + ex.Message);
            }
            return objMerchantDetailsJsonRetriveResponse;
        }
        public GetRaffleDetailsResponse GetRaffleDetails(GetRaffleDetailsRequest objGetRaffleDetailsRequest)
        {
            GetRaffleDetailsResponse ObjGetRaffleDetailsResponse = new GetRaffleDetailsResponse();
            try
            {
                ObjGetRaffleDetailsResponse = mobileAppBO.GetRaffleDetails(objGetRaffleDetailsRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(SERVICE_CLASS_NAME + " :: GetRaffleDetails() " + ex.Message);
            }
            return ObjGetRaffleDetailsResponse;
        }


        public SaveTranRaffleDetailsResponse SaveTranRaffleDetails(SaveTranRaffleDetailsRequest objSaveTranRaffleDetailsRequest)
        {
            SaveTranRaffleDetailsResponse ObjSaveTranRaffleDetailsResponse = new SaveTranRaffleDetailsResponse();
            try
            {
                ObjSaveTranRaffleDetailsResponse = mobileAppBO.SaveTranRaffleDetails(objSaveTranRaffleDetailsRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(SERVICE_CLASS_NAME + " :: GetRaffleDetails() " + ex.Message);
            }
            return ObjSaveTranRaffleDetailsResponse;
        }
        public SceneResponse SaveSceneCompleted(SceneRequest ObjSceneRequest)
        {
            SceneResponse objSceneResponse = null;
            try
            {
                objSceneResponse = mobileAppBO.SaveSceneCompleted(ObjSceneRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "SaveSceneCompleted()" + ex.Message);
            }
            return objSceneResponse;
        }

        public MenuRetrieveResponse GetUserMenuDetails(MenuRetrieveRequest ObjMenuRetrieveRequest)
        {
            MenuRetrieveResponse ObjMenuRetrieveResponse = null;
            try
            {
                ObjMenuRetrieveResponse = mobileAppBO.GetUserMenuDetails(ObjMenuRetrieveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "GetUserMenuDetails : " + ex.Message);
            }
            return ObjMenuRetrieveResponse;
        }

        public ProductSaveResponse SaveEGiftRequestRedington(ProductSaveRequest objProductSaveRequest)
        {
            ProductSaveResponse objProductSaveResponse = null;
            try
            {
                string FullPath = string.Empty, FileName = string.Empty;
                FullPath = HTTP_UPLOAD_FOLDER_PATH + PROFILE_PIC_FOLDER_PATH;
                //Display Image
                if (!string.IsNullOrEmpty(objProductSaveRequest.ProductSaveDetails.ProductImage))
                {
                    FileName = objProductSaveRequest.LoyaltyID + "_Claim_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".png";
                    System.Drawing.Image CustomerImage = UICommon.Base64ToImageConverter(objProductSaveRequest.ProductSaveDetails.ProductImage);

                    CustomerImage.Save((FullPath.Replace("~", "") + FileName), ImageFormat.Png);
                    objProductSaveRequest.ProductSaveDetails.ProductImage = PROFILE_PIC_FOLDER_PATH.Replace("\\", "/") + FileName;
                }
                objProductSaveResponse = mobileAppBO.SaveEGiftRequestRedington(objProductSaveRequest);

            }
            catch (Exception ex)
            {

                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " : SaveEGiftRequestMyEssilorRewards() : " + ex.Message);
            }

            return objProductSaveResponse;
        }

        public CheckIMIEExistencyRetrieveResponse CheckSerialNoExistency(CheckIMIEExistencyRetrieveRequest objCheckIMEIExistencyRetrieveRequest)
        {
            CheckIMIEExistencyRetrieveResponse ObjCheckIMIEExistencyRetrieveResponse = null;
            try
            {
                ObjCheckIMIEExistencyRetrieveResponse = mobileAppBO.CheckSerialNoExistency(objCheckIMEIExistencyRetrieveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "CheckSerialNoExistency : " + ex.Message);
            }
            return ObjCheckIMIEExistencyRetrieveResponse;
        }

        public CustEarnPointDetailsRetriveResponse GetCustomerEarnedPointDetails(CustEarnPointDetailsRetriveRequest objCustEarnPointDetailsRetriveRequest)
        {
            CustEarnPointDetailsRetriveResponse ObjCustEarnPointDetailsRetriveResponse = null;
            try
            {
                ObjCustEarnPointDetailsRetriveResponse = mobileAppBO.GetCustomerEarnedPointDetails(objCustEarnPointDetailsRetriveRequest);
                if (ObjCustEarnPointDetailsRetriveResponse != null && ObjCustEarnPointDetailsRetriveResponse.ObjCustomerEarnedPointDetailsList.Count > 0)
                {
                    var EXCEL = GetCustomerEarnedPointDetailsExcel(ObjCustEarnPointDetailsRetriveResponse.ObjCustomerEarnedPointDetailsList, objCustEarnPointDetailsRetriveRequest);

                    string base64String = Convert.ToBase64String(EXCEL, 0, EXCEL.Length);
                    ObjCustEarnPointDetailsRetriveResponse.EXCEL = base64String;
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "GetCustomerEarnedPointDetails : " + ex.Message);
            }
            return ObjCustEarnPointDetailsRetriveResponse;
        }

        public CustPerformanceDetailsRetriveResponse GetCustomerPerformanceDetails(CustPerformanceDetailsRetriveRequest objCustPerformanceDetailsRetriveRequest)
        {
            CustPerformanceDetailsRetriveResponse ObjCustPerformanceDetailsRetriveResponse = null;
            try
            {
                ObjCustPerformanceDetailsRetriveResponse = mobileAppBO.GetCustomerPerformanceDetails(objCustPerformanceDetailsRetriveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "GetCustomerPerformanceDetails : " + ex.Message);
            }
            return ObjCustPerformanceDetailsRetriveResponse;
        }

        public GetTDSCovidRedemptionDetailsResponse GetTDSCovidRedemptionDetails(GetTDSCovidRedemptionDetailsRequest ObjGetTDSCovidRedemptionDetailsRequest)
        {
            GetTDSCovidRedemptionDetailsResponse ObjGetTDSCovidRedemptionDetailsResponse = null;
            try
            {
                ObjGetTDSCovidRedemptionDetailsResponse = mobileAppBO.GetTDSCovidRedemptionDetails(ObjGetTDSCovidRedemptionDetailsRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "GetTDSCovidRedemptionDetails : " + ex.Message);
            }
            return ObjGetTDSCovidRedemptionDetailsResponse;
        }

        public CatalogueSaveResponse SaveCatalogueRedemptionDetailsForCovid(CatalogueSaveRequest ObjCatalogueSaveRequest)
        {
            CatalogueSaveResponse objCatalogueSaveResponse = null;
            try
            {
                string FullPath = string.Empty, FileName = string.Empty;
                FullPath = HTTP_UPLOAD_FOLDER_PATH + BANK_PIC_FOLDER_PATH;
                if (!string.IsNullOrEmpty(ObjCatalogueSaveRequest.PanImage) && ObjCatalogueSaveRequest.IsPanNewImage == true)
                {
                    FileName = ObjCatalogueSaveRequest.LoyaltyId + "_PanCard_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".png";
                    System.Drawing.Image PanImage = UICommon.Base64ToImageConverter(ObjCatalogueSaveRequest.PanImage);

                    PanImage.Save((FullPath.Replace("~", "") + FileName), ImageFormat.Png);
                    ObjCatalogueSaveRequest.PanImage = BANK_PIC_FOLDER_PATH.Replace("\\", "/") + FileName;
                }
                objCatalogueSaveResponse = mobileAppBO.SaveCatalogueRedemptionDetailsForCovid(ObjCatalogueSaveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " SaveCatalogueRedemptionDetailsForCovid() : " + ex.Message);
            }
            finally { }
            return objCatalogueSaveResponse;
        }

        public Byte[] GetCustomerEarnedPointDetailsExcel(List<CustomerEarnedPointDetails> lstCustomerEarnedPointDetails, CustEarnPointDetailsRetriveRequest objCustEarnPointDetailsRetriveRequest)
        {
            Byte[] bytes = null;
            try
            {
                ReportViewer rdlcCustomerList = new ReportViewer();
                rdlcCustomerList.LocalReport.DataSources.Clear();
                rdlcCustomerList.LocalReport.ReportPath = rdlcReportCustomerEarnedDetails;
                rdlcCustomerList.LocalReport.EnableHyperlinks = true;
                rdlcCustomerList.LocalReport.Refresh();
                ReportParameter p1 = new ReportParameter("FromDate", objCustEarnPointDetailsRetriveRequest.FromDate);
                ReportParameter p2 = new ReportParameter("ToDate", objCustEarnPointDetailsRetriveRequest.ToDate);
                rdlcCustomerList.LocalReport.SetParameters(new ReportParameter[] { p1, p2 });
                ReportDataSource TableDataSource = new ReportDataSource(ExportTableCustomerEarnedDetailsExcel, lstCustomerEarnedPointDetails);
                rdlcCustomerList.LocalReport.DataSources.Add(TableDataSource);
                //Export
                Warning[] warnings;
                string[] streamIds;
                string mimeType = string.Empty;
                string encoding = string.Empty;
                string extension = string.Empty;
                string fileName = "CustomerEarnedPointsDetails" + "_" + System.DateTime.Now.ToString("ddMMyyyy_hhmm");
                bytes = rdlcCustomerList.LocalReport.Render(UIConstants.EXCEL, null, out mimeType, out encoding, out extension, out streamIds, out warnings);
                //Now that you have all the bytes representing the EXCEL report, buffer it and send it to the client.
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetCustomerEarnedPointDetailsExcel() " + ex.Message);
            }
            return bytes;
        }
        public GetEventManagementDetailsResponse GetEventManagementDetails(GetEventManagementDetailsRequest ObjGetEventManagementDetailsRequest)
        {
            GetEventManagementDetailsResponse objGetEventManagementDetailsResponse = null;
            try
            {
                objGetEventManagementDetailsResponse = mobileAppBO.GetEventManagementDetails(ObjGetEventManagementDetailsRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " SaveCatalogueRedemptionDetailsForCovid() : " + ex.Message);
            }

            return objGetEventManagementDetailsResponse;
        }

        public RedemptionPlannerRetriveResponseJson GetPlannerAddedOrNotJson(RedemptionPlannerRetriveRequestJson objRedemptionPlannerRetriveRequestJson)
        {
            RedemptionPlannerRetriveResponseJson objRedemptionPlannerRetriveResponseJson = null;
            try
            {
                objRedemptionPlannerRetriveResponseJson = mobileAppBO.GetPlannerAddedOrNotJson(objRedemptionPlannerRetriveRequestJson);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetPlannerAddedOrNot() " + ex.Message);
            }
            return objRedemptionPlannerRetriveResponseJson;
        }

        public GstDetailsResponse GetGstDetails(GstDetailsRequest objGstDetailsRequest)
        {
            GstDetailsResponse objGstDetailsResponse = null;
            ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetGstDetails() " + objGstDetailsRequest.GstNumber);
            try
            {
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                string TokenUrl = TOKEN_URL;
                ArokiaIT.Skipper.Common.Utility.HttpRequestUtility.HttpRequestArguments httpRequestArguments = new ArokiaIT.Skipper.Common.Utility.HttpRequestUtility.HttpRequestArguments(TokenUrl, "POST", "application/json", false, "");
                httpRequestArguments.Headers = new WebHeaderCollection();

                httpRequestArguments.Headers.Add("x-api-key", GST_API_KEY);
                httpRequestArguments.Headers.Add("x-api-secret", GST_SECURITY_KEY);
                httpRequestArguments.Headers.Add("x-api-version", "Version 3.4.1");
                string GstToken = ArokiaIT.Skipper.Common.Utility.HttpRequestUtility.HttpRequest.MakePostRequest(httpRequestArguments);
                JObject response = JObject.Parse(GstToken);
                GstToken = response["access_token"].ToString();

                ServicePointManager.SecurityProtocol = (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                string GstURL = GST_DETAIL_URL + objGstDetailsRequest.GstNumber;
                ArokiaIT.Skipper.Common.Utility.HttpRequestUtility.HttpRequestArguments httpRequestArgumentGst = new ArokiaIT.Skipper.Common.Utility.HttpRequestUtility.HttpRequestArguments(GstURL, "", "application/json", false, "");
                httpRequestArgumentGst.Headers = new WebHeaderCollection();
                httpRequestArgumentGst.Headers.Add("Authorization", GstToken);
                httpRequestArgumentGst.Headers.Add("x-api-key", GST_API_KEY);
                httpRequestArgumentGst.Headers.Add("x-api-version", "Version 3.4.1");
                string ResponseMessage = ArokiaIT.Skipper.Common.Utility.HttpRequestUtility.HttpRequest.MakeGetRequest(httpRequestArgumentGst);

                //string ResponseMessage = "{\"transaction_id\":\"397cbf12-dac2-40ba-bcdd-84d4ecc83dbb\",\"code\":422,\"message\":\"Invalid GSTIN pattern\",\"timestamp\":1626847598808}".ToString();
                JObject objresponse = JObject.Parse(ResponseMessage);
                RootGst myDeserializedClass = JsonConvert.DeserializeObject<RootGst>(ResponseMessage);

                objGstDetailsResponse = new GstDetailsResponse();
                objGstDetailsResponse.code = myDeserializedClass.code.ToString();
                if (myDeserializedClass.code.ToString() == "200")
                {
                    objGstDetailsResponse.CompanyName = myDeserializedClass.data.tradeNam.ToString();
                    objGstDetailsResponse.GstNo = myDeserializedClass.data.gstin.ToString();
                    objGstDetailsResponse.Address = myDeserializedClass.data.pradr.addr.bnm + ' ' + myDeserializedClass.data.pradr.addr.st + ' ' + myDeserializedClass.data.pradr.addr.loc + ' ' + myDeserializedClass.data.pradr.addr.bno + ' ' + myDeserializedClass.data.pradr.addr.flno + ' ' + myDeserializedClass.data.pradr.addr.pncd.ToString();
                    objGstDetailsResponse.State = myDeserializedClass.data.pradr.addr.stcd.ToString();
                    objGstDetailsResponse.Dist = myDeserializedClass.data.pradr.addr.dst.ToString();
                    objGstDetailsResponse.City = myDeserializedClass.data.pradr.addr.city.ToString();
                    objGstDetailsResponse.PinCode = myDeserializedClass.data.pradr.addr.pncd.ToString();
                }
                else
                {
                    objGstDetailsResponse.Message = myDeserializedClass.message.ToString();
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetGstDetails() " + ex.Message);
            }
            return objGstDetailsResponse;
        }

        public ELearningEarningDetailsResponse GetEarningRedemptionDetailsForELearning(ELearningEarningDetailsRequest ObjELearningEarningDetailsRequest)
        {
            ELearningEarningDetailsResponse ObjELearningEarningDetailsResponse = null;
            try
            {
                ObjELearningEarningDetailsResponse = mobileAppBO.GetEarningRedemptionDetailsForELearning(ObjELearningEarningDetailsRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetEarningRedemptionDetailsForELearning() " + ex.Message);
            }
            return ObjELearningEarningDetailsResponse;
        }

        public GetClaimApprovalStatusResponse GetClaimApprovalStatusDetails(GetClaimApprovalStatusRequest ObjGetClaimApprovalStatusRequest)
        {
            GetClaimApprovalStatusResponse ObjGetClaimApprovalStatusResponse = null;
            try
            {
                ObjGetClaimApprovalStatusResponse = mobileAppBO.GetClaimApprovalStatusDetails(ObjGetClaimApprovalStatusRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetClaimApprovalStatusDetails() " + ex.Message);
            }
            return ObjGetClaimApprovalStatusResponse;
        }

        public ProductRetriveResponse GetProductBehaviourDetailsOnLoyaltyProgram(ProductRetriveRequest ObjProductRetriveRequest)
        {
            ProductRetriveResponse ObjProductRetriveResponse = null;
            try
            {
                ObjProductRetriveResponse = mobileAppBO.GetProductBehaviourDetailsOnLoyaltyProgram(ObjProductRetriveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetProductBehaviourDetailsOnLoyaltyProgram() " + ex.Message);
            }
            return ObjProductRetriveResponse;
        }

        public GetBonusCampaignRetriveResponseForMobileApp GetBonusCampaignDetails(GetBonusCampaignRetriveRequestForMobileApp ObjGetBonusCampaignRetriveRequestForMobileApp)
        {
            GetBonusCampaignRetriveResponseForMobileApp ObjGetBonusCampaignRetriveResponseForMobileApp = null;
            try
            {
                ObjGetBonusCampaignRetriveResponseForMobileApp = mobileAppBO.GetBonusCampaignDetails(ObjGetBonusCampaignRetriveRequestForMobileApp);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetBonusCampaignDetails() " + ex.Message);
            }
            return ObjGetBonusCampaignRetriveResponseForMobileApp;
        }

        public ProductRetriveResponseAPI BindProductDetailsMobile(ProductRetriveRequest objProductRetriveRequest)
        {
            ProductRetriveResponseAPI objProductRetriveResponseAPI = null;
            try
            {

                objProductRetriveResponseAPI = mobileAppBO.BindProductDetailsMobile(objProductRetriveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " BindProductDetailsMobile : " + ex.Message);
            }

            return objProductRetriveResponseAPI;
        }
        public ProductSaveResponse UpdateCustomerCartDetails(BindCustomerCartRetrieveRequestApi objCartProductRetriveRequest)
        {
            ProductSaveResponse objProductSaveResponse = null;
            try
            {
                objProductSaveResponse = mobileAppBO.UpdateCustomerCartDetails(objCartProductRetriveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " UpdateCustomerCartDetails : " + ex.Message);
            }

            return objProductSaveResponse;
        }

        public string EncryptionToDecryption(string value)
        {
            return value = SecuityManager.Decryption(value, SECREATE_KEY);
        }

        public string DecryptionToEncryption(string value)
        {
            return value = SecuityManager.Encryption(value, SECREATE_KEY);
        }

        public QRCodeSaveResponse CheckQRCodeExistanceForShopboy(QRCodeValidationRequest QRCodeValidationRequest)
        {
            QRCodeSaveResponse objQRCodeSaveResponse = null;
            try
            {
                objQRCodeSaveResponse = mobileAppBO.CheckQRCodeExistanceForShopboy(QRCodeValidationRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " CheckQRCodeExistanceForShopboy : " + ex.Message);
            }
            return objQRCodeSaveResponse;
        }

        public CatalogueCartSaveResponse SaveCatalogueCartDetailsMobile(CatalogueCartSaveRequest objCatalogueCartSaveRequest)
        {
            CatalogueCartSaveResponse objCatalogueCartSaveResponse = null;
            try
            {
                objCatalogueCartSaveResponse = mobileAppBO.SaveCatalogueCartDetailsMobile(objCatalogueCartSaveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " SaveCatalogueCartDetailsMobile() " + ex.Message);
            }
            return objCatalogueCartSaveResponse;
        }

        public CatalogueCartSaveResponse GetCatalogueCartDetailsMobile(CatalogueCartSaveRequest objCatalogueCartGetRequest)
        {
            CatalogueCartSaveResponse objCatalogueCartGetResponse = null;
            try
            {
                objCatalogueCartGetRequest.Domain = DOMAIN;
                objCatalogueCartGetResponse = mobileAppBO.GetCatalogueCartDetailsMobile(objCatalogueCartGetRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetCatalogueCartDetailsMobile() " + ex.Message);
            }
            return objCatalogueCartGetResponse;
        }

        public int CheckCustomerExistancyAndVerificationJson(LocationSaveRequestJson ObjLocationSaveRequest)
        {
            LocationSaveResponseJson ObjLocationSaveResponse = null;
            int ReturnValue = 0;
            try
            {
                ObjLocationSaveResponse = mobileAppBO.CheckEmailMobileExistsMobileAppJson(ObjLocationSaveRequest);
                ReturnValue = ObjLocationSaveResponse.ReturnValue;
            }
            catch (Exception ex)
            {

                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " CheckCustomerExistancyAndVerifications() ::" + ex.Message);
            }
            return ReturnValue;
        }

        public AreaRetrieveResponse GetAreaDetails(AreaRetrieveRequest objAreaRetrieveRequest)
        {
            AreaRetrieveResponse objAreaRetrieveResponse = null;
            try
            {
                objAreaRetrieveResponse = mobileAppBO.GetAreaDetails(objAreaRetrieveRequest);
            }
            catch (Exception ex)
            {

                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " : GetAreaDetails() : " + ex.Message);
            }
            return objAreaRetrieveResponse;
        }

        private void SendSuccessfullRegistrationUpdationAlerts(string CustomerName, string Email, string LoyaltyId, int countryId, string TemplateName, string Status)
        {
            try
            {

                AttributesRetrieveRequest objAttributesRetrieveRequest = new AttributesRetrieveRequest();
                objAttributesRetrieveRequest.ActionType = Convert.ToInt32(Enumeration_Loyalty.SmsEmailTemplates.Query_ForUserOnReassignment);
                objAttributesRetrieveRequest.UserType = Email;
                objAttributesRetrieveRequest.RoleIDs = UIConstants.VALUE_TWO;
                objAttributesRetrieveRequest.HelpTopicID = countryId;
                AttributesRetrieveResponse objAttributesRetrieveResponse = mobileAppBO.GetAttributeDetails(objAttributesRetrieveRequest);
                if (objAttributesRetrieveResponse != null && objAttributesRetrieveResponse.lstAttributesDetails != null && objAttributesRetrieveResponse.lstAttributesDetails.Count > 0)
                {
                    var str = String.Join(",", objAttributesRetrieveResponse.lstAttributesDetails.Select(p => p.AttributeValue));
                    SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                    ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(TemplateName);
                    ObjSmsEmailRetrieveRequest.Type = UIConstants.EMAIL;
                    ObjSmsEmailRetrieveRequest.UserName = MERCHANT_USERNAME;
                    SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse = mobileAppBO.GetEmailSmsTemplate(ObjSmsEmailRetrieveRequest);
                    for (int i = 0; i < ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count; i++)
                    {
                        List<AlertTemplateDynamicContent> DynamicTemplate = new List<AlertTemplateDynamicContent>(){
                            new AlertTemplateDynamicContent ("[Date]",DateTime.Now.ToString("dd/MM/yyyy")),
                            new AlertTemplateDynamicContent ("[Email ID]",Email),
                            new AlertTemplateDynamicContent ("[Company Legal Name]",CustomerName),
                            new AlertTemplateDynamicContent ("[Account Status]",Status),
                            new AlertTemplateDynamicContent("[MemberId]", LoyaltyId)

                            };

                        AlertUtiltityParameters alertUtiltityParameters = new AlertUtiltityParameters
                       (UIConstants.EMAIL, str, DynamicTemplate, ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i]);
                        alertUtiltityParameters.IsGetRequest = true;

                        var Result = SendAlertUtility.SendAlert(alertUtiltityParameters);
                    }
                }




            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " SendSuccessfullRegistrationUpdationAlerts() : " + ex.Message);
            }
        }

        public ProductSaveResponse SaveEGiftRequestTurtlewax(ProductSaveRequest objProductSaveRequest)
        {
            ProductSaveResponse objProductSaveResponse = null;
            try
            {
                string FullPath = string.Empty, FileName = string.Empty;
                FullPath = HTTP_UPLOAD_FOLDER_PATH + PROFILE_PIC_FOLDER_PATH;
                //Display Image
                if (objProductSaveRequest.ProductSaveDetails.ProductImage != null)
                {
                    FileName = objProductSaveRequest.LoyaltyID + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".png";
                    System.Drawing.Image CustomerImage = UICommon.Base64ToImageConverter(objProductSaveRequest.ProductSaveDetails.ProductImage);

                    CustomerImage.Save((FullPath.Replace("~", "") + FileName), ImageFormat.Png);
                    objProductSaveRequest.ProductSaveDetails.ProductImage = PROFILE_PIC_FOLDER_PATH.Replace("\\", "/") + FileName;

                }
                objProductSaveResponse = mobileAppBO.SaveEGiftRequestTurtlewax(objProductSaveRequest);

                if (objProductSaveResponse != null)
                {
                    string[] arg = objProductSaveResponse.ReturnMessage.Split('~');
                    string ReturnValue = arg[0];

                }

            }
            catch (Exception ex)
            {

                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " : SaveEGiftRequest_Wavin() : " + ex.Message);
            }

            return objProductSaveResponse;
        }

        public CustomerActiveResponse CustomerAccountDelete(CustomerActiveRequest ObjCustomerActiveRequest)
        {
            CustomerActiveResponse ObjCustomerActiveResponse = null;
            try
            {
                if (ObjCustomerActiveRequest.userid != null)
                    ObjCustomerActiveResponse = mobileAppBO.CustomerAccountDelete(ObjCustomerActiveRequest);
            }
            catch (Exception ex)
            {

                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " : CustomerAccountDelete() : " + ex.Message);
            }
            return ObjCustomerActiveResponse;
        }

        public GetOrderDetailsResponse GetCustomerOrderDetails(GetOrderDetailsRequest objGetOrderDetailsRequest)
        {
            GetOrderDetailsResponse objGetOrderDetailsResponse = null;
            try
            {
                objGetOrderDetailsResponse = mobileAppBO.GetCustomerOrderDetails(objGetOrderDetailsRequest);
            }
            catch (Exception ex)
            {

                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " : GetCustomerOrderDetails() : " + ex.Message);
            }
            return objGetOrderDetailsResponse;
        }

    }

    [Obsolete("Replaced SMTP class with DataBaseSMTP")]
    public class SMTP
    {
        public static List<string> GetSmtp(string MerchantName)
        {
            List<string> lstsmtp = new List<string>();
            try
            {
                SMTP_SMS_DetailsRetriveRequest objSMTP_SMS_DetailsRetriveRequest = new SMTP_SMS_DetailsRetriveRequest();
                MobileAppBO objMobileAppBO = new MobileAppBO();
                objSMTP_SMS_DetailsRetriveRequest.MerchantName = MerchantName;
                SMTP_SMS_DetailsRetriveResponse objSMTP_SMS_DetailsRetriveResponse = objMobileAppBO.GetSMTPDetailsForSendMailByMerchantId(objSMTP_SMS_DetailsRetriveRequest);
                {
                    if (objSMTP_SMS_DetailsRetriveResponse.lstSMSServiceprovider.Count > 0)
                    {
                        objSMTP_SMS_DetailsRetriveResponse.lstSMSServiceprovider = objSMTP_SMS_DetailsRetriveResponse.lstSMSServiceprovider;
                        lstsmtp.Add(Convert.ToString(objSMTP_SMS_DetailsRetriveResponse.lstSMSServiceprovider[0].SMTPAddress));
                        lstsmtp.Add(Convert.ToString(objSMTP_SMS_DetailsRetriveResponse.lstSMSServiceprovider[0].SMTPUserName));
                        lstsmtp.Add(Convert.ToString(objSMTP_SMS_DetailsRetriveResponse.lstSMSServiceprovider[0].SMTPPassword));
                        lstsmtp.Add(Convert.ToString(objSMTP_SMS_DetailsRetriveResponse.lstSMSServiceprovider[0].FromEmail));
                        lstsmtp.Add(Convert.ToString(objSMTP_SMS_DetailsRetriveResponse.lstSMSServiceprovider[0].SMSSender));
                        lstsmtp.Add(Convert.ToString(objSMTP_SMS_DetailsRetriveResponse.lstSMSServiceprovider[0].SMSWorkingKey));
                        lstsmtp.Add(Convert.ToString(objSMTP_SMS_DetailsRetriveResponse.lstSMSServiceprovider[0].SmsURL));
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstsmtp;
        }
    }

    public class DataBaseSMTP
    {
        public static Dictionary<string, string> GetDBSmtp(string MerchantName)
        {
            Dictionary<string, string> smtpDetails = new Dictionary<string, string>();
            SMTP_SMS_DetailsRetriveRequest objSMTP_SMS_DetailsRetriveRequest = new SMTP_SMS_DetailsRetriveRequest();
            MobileAppBO objMobileAppBO = new MobileAppBO();
            objSMTP_SMS_DetailsRetriveRequest.MerchantName = MerchantName;
            SMTP_SMS_DetailsRetriveResponse objSMTP_SMS_DetailsRetriveResponse = objMobileAppBO.GetSMTPDetailsForSendMailByMerchantId(objSMTP_SMS_DetailsRetriveRequest);
            {
                if (objSMTP_SMS_DetailsRetriveResponse.lstSMSServiceprovider.Count > 0)
                {
                    objSMTP_SMS_DetailsRetriveResponse.lstSMSServiceprovider = objSMTP_SMS_DetailsRetriveResponse.lstSMSServiceprovider;
                    smtpDetails.Add("SMTPAddress", objSMTP_SMS_DetailsRetriveResponse.lstSMSServiceprovider[0].SMTPAddress);
                    smtpDetails.Add("SMTPUserName", objSMTP_SMS_DetailsRetriveResponse.lstSMSServiceprovider[0].SMTPUserName);
                    smtpDetails.Add("SMTPPassword", objSMTP_SMS_DetailsRetriveResponse.lstSMSServiceprovider[0].SMTPPassword);
                    smtpDetails.Add("FromEmail", objSMTP_SMS_DetailsRetriveResponse.lstSMSServiceprovider[0].FromEmail);
                    smtpDetails.Add("TestEmail", objSMTP_SMS_DetailsRetriveResponse.lstSMSServiceprovider[0].TestEmail);
                    smtpDetails.Add("SMSSender", objSMTP_SMS_DetailsRetriveResponse.lstSMSServiceprovider[0].SMSSender);
                    smtpDetails.Add("SMSWorkingKey", objSMTP_SMS_DetailsRetriveResponse.lstSMSServiceprovider[0].SMSWorkingKey);
                    smtpDetails.Add("SmsURL", objSMTP_SMS_DetailsRetriveResponse.lstSMSServiceprovider[0].SmsURL);
                    smtpDetails.Add("SSLInfo", objSMTP_SMS_DetailsRetriveResponse.lstSMSServiceprovider[0].SSLInfo);
                    smtpDetails.Add("PortNumber", objSMTP_SMS_DetailsRetriveResponse.lstSMSServiceprovider[0].PortNumber);
                }
            }
            return smtpDetails;
        }
    }
}
