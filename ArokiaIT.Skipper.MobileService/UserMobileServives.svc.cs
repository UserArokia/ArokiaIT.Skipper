using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using ArokiaIT.Skipper.Common.Contract.ServiceContracts;
using ArokiaIT.Skipper.Common.Contract.DataContracts;
using ArokiaIT.Skipper.Common.Utility;
using ArokiaIT.Framework.Architecture;
using ArokiaIT.Skipper.Business.MobileApp;
using System.Collections.Generic;
using System.Configuration;
using ArokiaIT.Skipper.Common.Contract.Enumerations;
using ArokiaIT.Skipper.Common.Utility.AlertUtility;
using Microsoft.Reporting.WebForms;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Globalization;
using ArokiaIT.Skipper.Common.Utility.HttpRequestUtility;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace ArokiaIT.Skipper.MobileService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class UserMobileServives : ISeMobileApi
    {
        #region Variables
        private const string SERVICE_CLASS_NAME = "UserMobileServives.svc.cs";
        private string MERCHANT_USERNAME = ConfigurationManager.AppSettings["MerchantUsername"];
        private string PROFILE_PIC_CONCAT_PATH = "~/UploadFiles/CustomerImage/";
        public const string PROFILE_PIC_FOLDER_PATH = @"~\UploadFiles\CustomerImage\";
        private const string IDENTITY_FOLDER_PATH = @"~\UploadFiles\IdentificationProof\";
        private const string BANK_PIC_FOLDER_PATH = @"~\UploadFiles\BankImageUpload\";
        private static string HTTP_UPLOAD_FOLDER_PATH = ConfigurationManager.AppSettings["UPLOAD_FOLDER_PATH"];
        private const string PAGE_PREFIX = "DFP";
        private const string TO_NAME = "[Client name]";
        private const string PIN = " [PIN]";
        private const string UPDATE_CHANGED_PASSWORD = "UpdateChangedPassword";
        private const string CHECK_IS_USER = "CheckIsUser";
        private const string MERCHANT_BY_LOCATION = "GetMerchantDetailsByLocation";
        private const string UPDATE_CUSTOMER_PASSWORD = "UpdateCustomerPassword";
        private const string CHECK_LOLALTYID_EXISTS = "CheckLoyaltyIdExists";
        private const string LOGIN = "Login";
        private static string DOMAIN = ConfigurationManager.AppSettings["Domain"];
        private const string ExportTableName = "CustomerrPointsTrend";
        private static string rdlcReportPath = HTTP_UPLOAD_FOLDER_PATH + @"\RDLCReports\CustomerPointsTrend.rdlc";
        private static string GOOGLE_API_KEY = ConfigurationManager.AppSettings["GoogleAPIKey"];
        private const string SAVE_LOG_DETAILS = "SaveLogDetails";
        private const string RECEIPT_IMAGE_FOLDER_PATH = @"~\UploadFiles\ReceiptImage\";

        private const string IDENTITY_PROOF_PIC_FOLDER_PATH = @"~\UploadFiles\IdentificationProof\";
        private readonly string PAN_API_KEY = ConfigurationManager.AppSettings["PAN_Apikey"];
        private readonly string PAN_API_VERSION = ConfigurationManager.AppSettings["PanApiVersion"];
        private readonly string PAN_API_SECRET_KEY = ConfigurationManager.AppSettings["PAN_ApiSecrectkey"];
        private readonly string PAN_TOKEN_GENERATE_URL = ConfigurationManager.AppSettings["PANTokenGenerateUrl"];
        private readonly string PAN_VALIDATE_URL = ConfigurationManager.AppSettings["PANValidationURL"];

        //=====================================*push notification constants*==========================================
        //private string PushType = ConfigurationManager.AppSettings["PushType"].ToString();
        //private string alert = ConfigurationManager.AppSettings["PushAlert"].ToString();
        //private string PromotionImageUrl = ConfigurationManager.AppSettings["PromotionImageUrl"].ToString();
        //private string serverKey = ConfigurationManager.AppSettings["PushWorkingKey"].ToString();
        //private string vibrate = ConfigurationManager.AppSettings["PushVibrate"].ToString(); // "true";
        //private string senderId = ConfigurationManager.AppSettings["PushSenderID"].ToString();
        //private string To = ConfigurationManager.AppSettings["PushTo"].ToString(); //"/topics/mytopic";
        //====================================*end of pusg notification constants*====================================

        // Added Logical Conditions to handle the Image Path if the image has only image name in GetCustomerDetails method
        #endregion Variables

        public CustomerRetrieveResponse GetCustomerDetails(CustomerRetrieveRequest ObjCustomerRetrieveRequest)
        {
            CustomerRetrieveResponse objCustomerRetrieveResponse = null;
            try
            {
                var chars = "~/";
                SeMobileAppBO objSeMobileAppBO = new SeMobileAppBO();
                ObjCustomerRetrieveRequest.Domain = DOMAIN;
                objCustomerRetrieveResponse = objSeMobileAppBO.GetCustomerDetails(ObjCustomerRetrieveRequest);
                if (objCustomerRetrieveResponse != null && objCustomerRetrieveResponse.CustomerBasicInfoList != null && objCustomerRetrieveResponse.CustomerBasicInfoList.Count > 0)
                {
                    for (int k = 0; k <= objCustomerRetrieveResponse.CustomerBasicInfoList.Count - 1; k++)
                    {
                        if (!string.IsNullOrEmpty(objCustomerRetrieveResponse.CustomerBasicInfoList[k].ProfilePicture))
                        {
                            if (objCustomerRetrieveResponse.CustomerBasicInfoList[k].ProfilePicture.StartsWith(PROFILE_PIC_CONCAT_PATH + PROFILE_PIC_CONCAT_PATH))
                                objCustomerRetrieveResponse.CustomerBasicInfoList[k].ProfilePicture = objCustomerRetrieveResponse.CustomerBasicInfoList[k].ProfilePicture.Remove(0, 28);

                            if (!objCustomerRetrieveResponse.CustomerBasicInfoList[k].ProfilePicture.StartsWith(PROFILE_PIC_CONCAT_PATH))
                            {
                                string profilePicturePatch = objCustomerRetrieveResponse.CustomerBasicInfoList[k].ProfilePicture;

                                if (objCustomerRetrieveResponse.CustomerBasicInfoList[k].ProfilePicture.StartsWith(PROFILE_PIC_CONCAT_PATH.TrimStart(chars.ToCharArray())))
                                    objCustomerRetrieveResponse.CustomerBasicInfoList[k].ProfilePicture = "~/" + profilePicturePatch;
                                else
                                    objCustomerRetrieveResponse.CustomerBasicInfoList[k].ProfilePicture = PROFILE_PIC_CONCAT_PATH + profilePicturePatch;
                            }
                        }
                        if (!string.IsNullOrEmpty(objCustomerRetrieveResponse.CustomerBasicInfoList[k].TahasilImage))
                        {
                            string TahasilImage = objCustomerRetrieveResponse.CustomerBasicInfoList[k].TahasilImage;

                            if (objCustomerRetrieveResponse.CustomerBasicInfoList[k].TahasilImage.StartsWith(BANK_PIC_FOLDER_PATH.TrimStart(chars.ToCharArray())))
                                objCustomerRetrieveResponse.CustomerBasicInfoList[k].TahasilImage = "~/" + TahasilImage;
                            else
                                objCustomerRetrieveResponse.CustomerBasicInfoList[k].TahasilImage = BANK_PIC_FOLDER_PATH + TahasilImage;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "GetCustomerDetails : " + ex.Message);
            }
            return objCustomerRetrieveResponse;
        }

        public LoyaltyUserReatrieveResponse GetAsmSeDetails(LoyaltyUserRetriveRequest objLoyaltyUserRetriveRequest)
        {
            LoyaltyUserReatrieveResponse objLoyaltyUserReatrieveResponse = null;
            try
            {
                SeMobileAppBO objSeMobileAppBO = new SeMobileAppBO();
                objLoyaltyUserReatrieveResponse = objSeMobileAppBO.GetAsmSeDetails(objLoyaltyUserRetriveRequest);
            }
            catch (Exception ex)
            {

                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + ": GetAsmSeDetails : " + ex.Message);
            }
            return objLoyaltyUserReatrieveResponse;
        }

        public CustomerRetrieveResponse GetAsmSeMappingDetails(CustomerRetrieveRequest ObjCustomerRetrieveRequest)
        {
            CustomerRetrieveResponse objCustomerRetrieveResponse = null;
            try
            {
                SeMobileAppBO objSeMobileAppBO = new SeMobileAppBO();
                objCustomerRetrieveResponse = objSeMobileAppBO.GetAsmSeMappingDetails(ObjCustomerRetrieveRequest);
            }
            catch (Exception ex)
            {

                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + ": GetAsmSeMappingDetails() : " + ex.Message);
            }
            return objCustomerRetrieveResponse;
        }

        public CustomerSaveResponse SaveCustomerDetails(CustomerSaveRequest ObjCustomerSaveRequest)
        {
            CustomerSaveResponse objCustomerSaveResponse = null;
            try
            {
                string PlainPassword = string.Empty, FullPath = string.Empty; var FileName = string.Empty;
                ObjCustomerSaveRequest.ObjCustomer.Domain = DOMAIN;
                if (ObjCustomerSaveRequest.ActionType == Convert.ToInt32(UIConstants.VALUE_TWO) && DOMAIN != UIConstants.SKLP)
                {
                    FullPath = HTTP_UPLOAD_FOLDER_PATH + PROFILE_PIC_FOLDER_PATH;
                    //Display Image
                    if (!string.IsNullOrEmpty(ObjCustomerSaveRequest.ObjCustomerDetails.ProfilePicture) && ObjCustomerSaveRequest.ObjCustomerDetails.IsNewProfilePicture)
                    {
                        FileName = ObjCustomerSaveRequest.ObjCustomer.LoyaltyId + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".png";
                        System.Drawing.Image CustomerImage = UICommon.Base64ToImageConverter(ObjCustomerSaveRequest.ObjCustomerDetails.ProfilePicture);

                        CustomerImage.Save((FullPath.Replace("~", "") + FileName), System.Drawing.Imaging.ImageFormat.Png);
                        ObjCustomerSaveRequest.ObjCustomerDetails.ProfilePicture = PROFILE_PIC_FOLDER_PATH.Replace("\\", "/") + FileName;
                    }
                    //Display Image
                }
                else if (ObjCustomerSaveRequest.ActionType == Convert.ToInt32(UIConstants.VALUE_ONE) && (DOMAIN == UIConstants.SKLP || DOMAIN == UIConstants.LINC || DOMAIN == UIConstants.WAVIN))
                {
                    FullPath = HTTP_UPLOAD_FOLDER_PATH + PROFILE_PIC_FOLDER_PATH;
                    //Display Image
                    if (!string.IsNullOrEmpty(ObjCustomerSaveRequest.ObjCustomerDetails.ProfilePicture) && ObjCustomerSaveRequest.ObjCustomerDetails.IsNewProfilePicture)
                    {

                        FileName = ObjCustomerSaveRequest.ObjCustomer.LoyaltyId + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".png";
                        bool Result = UICommon.SaveBase64StringIntoFile(FullPath.Replace("~", ""), ObjCustomerSaveRequest.ObjCustomerDetails.ProfilePicture, FileName);
                        ObjCustomerSaveRequest.ObjCustomerDetails.ProfilePicture = PROFILE_PIC_FOLDER_PATH.Replace("\\", "/") + FileName;
                    }
                    //Display Image
                }

                #region Identification Proof
                if (ObjCustomerSaveRequest.ActionType == Convert.ToInt32(UIConstants.VALUE_THREE))
                {
                    FullPath = HTTP_UPLOAD_FOLDER_PATH + IDENTITY_FOLDER_PATH;

                    if (ObjCustomerSaveRequest.lstIdentityInfo != null)
                    {
                        if (ObjCustomerSaveRequest.lstIdentityInfo.Count > 0)
                        {
                            int ImageNo = 1;



                            foreach (CustomerIdentityInfo CustomerIdentityInfo in ObjCustomerSaveRequest.lstIdentityInfo)
                            {
                                if (!string.IsNullOrEmpty(CustomerIdentityInfo.IdentityDocument) && CustomerIdentityInfo.IsNewIdentity)
                                {
                                    FileName = DateTime.Now.ToString("ddMMyyyyhhmm")
                                                + "_" + CustomerIdentityInfo.IdentityType
                                                + ImageNo + ObjCustomerSaveRequest.ObjCustomer.LoyaltyId + ".png";

                                    //System.Drawing.Image IdentityImg = UICommon.Base64ToImageConverter(CustomerIdentityInfo.IdentityDocument);

                                    //IdentityImg.Save((FullPath.Replace("~", "") + FileName), System.Drawing.Imaging.ImageFormat.Png);

                                    bool result = UICommon.SaveBase64StringIntoFile(FullPath.Replace("~", ""), CustomerIdentityInfo.IdentityDocument, FileName);

                                    if (DOMAIN == UIConstants.CPML)
                                        CustomerIdentityInfo.IdentityDocument = FileName;
                                    else
                                        CustomerIdentityInfo.IdentityDocument = IDENTITY_FOLDER_PATH.Replace("\\", "/") + FileName;
                                    ImageNo += ImageNo;
                                }
                                else if (!string.IsNullOrEmpty(CustomerIdentityInfo.IdentityDocument) && !CustomerIdentityInfo.IsNewIdentity)
                                    CustomerIdentityInfo.IdentityDocument = CustomerIdentityInfo.IdentityDocument.Replace("\\", "/");

                                ImageNo += ImageNo;
                                FileName = string.Empty;
                            }
                        }
                    }
                }
                #endregion Identification Proof

                if (ObjCustomerSaveRequest.ActionType == Convert.ToInt32(UIConstants.VALUE_SEVEN))
                {
                    FullPath = HTTP_UPLOAD_FOLDER_PATH + BANK_PIC_FOLDER_PATH;
                    //Bank Passbook Image
                    if (!string.IsNullOrEmpty(ObjCustomerSaveRequest.ObjCustomer.BankPassbookImage) && ObjCustomerSaveRequest.ObjCustomer.IsBankPassbookNewImage == true)
                    {
                        FileName = ObjCustomerSaveRequest.ObjCustomer.LoyaltyId + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".png";
                        System.Drawing.Image BankImage = UICommon.Base64ToImageConverter(ObjCustomerSaveRequest.ObjCustomer.BankPassbookImage);

                        BankImage.Save((FullPath.Replace("~", "") + FileName), System.Drawing.Imaging.ImageFormat.Png);
                        ObjCustomerSaveRequest.ObjCustomer.BankPassbookImage = BANK_PIC_FOLDER_PATH.Replace("\\", "/") + FileName;
                    }
                    //Bank Passbook Image
                }
                if (ObjCustomerSaveRequest.ActionType == Convert.ToInt32(UIConstants.VALUE_ONE)
                    || ObjCustomerSaveRequest.ActionType == Convert.ToInt32(UIConstants.VALUE_SIX))
                {
                    PlainPassword = UICommon.GetRandomNumber();
                    ObjCustomerSaveRequest.ObjCustomer.Password = Security.EncryptPassword(PlainPassword);
                }
                if (DOMAIN == UIConstants.WAVIN)
                {
                    ObjCustomerSaveRequest.ObjCustomer.Password = Security.EncryptPassword("654321");
                    FullPath = HTTP_UPLOAD_FOLDER_PATH + BANK_PIC_FOLDER_PATH;
                    //Bank Passbook Image
                    if (ObjCustomerSaveRequest.ObjCustomerOfficalInfo != null && ObjCustomerSaveRequest.ObjCustomerOfficalInfo.TahasilImage != null)
                    {
                        if (!string.IsNullOrEmpty(ObjCustomerSaveRequest.ObjCustomerOfficalInfo.TahasilImage))
                        {
                            FileName = ObjCustomerSaveRequest.ObjCustomer.LoyaltyId + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".png";
                            UICommon.SaveBase64StringIntoFile(FullPath.Replace("~", ""), ObjCustomerSaveRequest.ObjCustomerOfficalInfo.TahasilImage, FileName);
                            ObjCustomerSaveRequest.ObjCustomerOfficalInfo.TahasilImage = BANK_PIC_FOLDER_PATH.Replace("\\", "/") + FileName;
                        }
                    }
                }
                if (ObjCustomerSaveRequest.ObjCustomer != null)
                {
                    if (!string.IsNullOrEmpty(ObjCustomerSaveRequest.ObjCustomer.JNomineeDOB) && (ObjCustomerSaveRequest.ObjCustomer.JNomineeDOB != Convert.ToString("1/1/0001")))
                        ObjCustomerSaveRequest.ObjCustomer.NomineeDOB = Convert.ToDateTime(ObjCustomerSaveRequest.ObjCustomer.JNomineeDOB);
                    if (!string.IsNullOrEmpty(ObjCustomerSaveRequest.ObjCustomer.JDOB) && (ObjCustomerSaveRequest.ObjCustomer.JDOB != Convert.ToString("1/1/0001")))
                        ObjCustomerSaveRequest.ObjCustomer.DOB = Convert.ToDateTime(ObjCustomerSaveRequest.ObjCustomer.JDOB);
                }
                if (ObjCustomerSaveRequest.ObjCustomerDetails != null)
                    if (!string.IsNullOrEmpty(ObjCustomerSaveRequest.ObjCustomerDetails.JAnniversary) && (ObjCustomerSaveRequest.ObjCustomerDetails.JAnniversary != Convert.ToString("1/1/0001")))
                        ObjCustomerSaveRequest.ObjCustomerDetails.Anniversary = Convert.ToDateTime(ObjCustomerSaveRequest.ObjCustomerDetails.JAnniversary);

                objCustomerSaveResponse = new SeMobileAppBO().SaveCustomerDetails(ObjCustomerSaveRequest);

                if (objCustomerSaveResponse != null)
                {
                    if (!string.IsNullOrEmpty(objCustomerSaveResponse.ReturnMessage))
                    {
                        if (objCustomerSaveResponse.ReturnMessage.Contains(":"))
                        {
                            string[] Results = objCustomerSaveResponse.ReturnMessage.Split(':');
                            if (Results[0] == UIConstants.VALUE_ONE)
                            {
                                if (ObjCustomerSaveRequest.ActionType == Convert.ToInt32(UIConstants.VALUE_ONE) && DOMAIN == UIConstants.GVLC && ObjCustomerSaveRequest.ObjCustomer.CustomerId == 0) //Customer Enrollment
                                    sendCustomerWelcomeSMS(ObjCustomerSaveRequest.ObjCustomer.FirstName, ObjCustomerSaveRequest.ObjCustomer.Mobile);

                                else if (ObjCustomerSaveRequest.ActionType == Convert.ToInt32(UIConstants.VALUE_ONE) && (DOMAIN == UIConstants.SKLP || DOMAIN == UIConstants.WAVIN) && ObjCustomerSaveRequest.ObjCustomer.CustomerId == 0) //Customer Enrollment
                                    SendCustomerCredentialSMS(ObjCustomerSaveRequest.ObjCustomer.FirstName, PlainPassword, ObjCustomerSaveRequest.ObjCustomer.Mobile, ObjCustomerSaveRequest.ObjCustomer.LoyaltyId);

                                else if (ObjCustomerSaveRequest.ActionType == Convert.ToInt32(UIConstants.VALUE_SIX))//CustomerVarification
                                {
                                    if (ObjCustomerSaveRequest.VerifiedStatus == 1 || ObjCustomerSaveRequest.VerifiedStatus == 4)
                                        if (DOMAIN == UIConstants.WAVIN)
                                            SendCustomerCredentialSMS(ObjCustomerSaveRequest.ObjCustomer.FirstName, PlainPassword, ObjCustomerSaveRequest.ObjCustomer.Mobile, ObjCustomerSaveRequest.ObjCustomer.LoyaltyId);
                                        else
                                            SendCustomerCredentialSMS(ObjCustomerSaveRequest.ObjCustomer.FirstName, PlainPassword, ObjCustomerSaveRequest.ObjCustomer.LoyaltyId, ObjCustomerSaveRequest.ObjCustomer.LoyaltyId);
                                }
                                else if (ObjCustomerSaveRequest.ActionType == Convert.ToInt32(UIConstants.VALUE_TWO) && DOMAIN == UIConstants.GVLC && ObjCustomerSaveRequest.ObjCustomer.VerifiedStatus == UIConstants.VALUE_FIVE) //Customer Enrollment
                                    SendMissCallNumberSMS(ObjCustomerSaveRequest.ObjCustomer.Mobile); //Send Miss Call Number to Enrolled customer

                                objCustomerSaveResponse.ReturnValue = 1;// Success
                            }
                        }
                        else
                            objCustomerSaveResponse.ReturnValue = -1;// Failure
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + ": SaveCustomerDetails() : " + ex.Message);
            }
            return objCustomerSaveResponse;
        }

        public CustomerSaveResponse SaveAsmSeMappingDetails(CustomerSaveRequest ObjCustomerSaveRequest)
        {
            CustomerSaveResponse objCustomerSaveResponse = null;
            try
            {
                string seName = string.Empty,
                        seMob = string.Empty,
                        asmName = string.Empty,
                        asmEmail = string.Empty,
                        customerMobile = string.Empty,
                        customerName = string.Empty;


                SeMobileAppBO objSeMobileAppBO = new SeMobileAppBO();
                objCustomerSaveResponse = objSeMobileAppBO.SaveAsmSeMappingDetails(ObjCustomerSaveRequest);
                if (objCustomerSaveResponse != null)
                {
                    if (objCustomerSaveResponse.ReturnMessage.Contains("~"))
                    {
                        string[] Results = objCustomerSaveResponse.ReturnMessage.Split('~');
                        seName = Results[1];
                        seMob = Results[2];
                        asmName = Results[3];
                        asmEmail = Results[4];
                        customerMobile = Results[5];
                        customerName = Results[6];

                        if (Results[0] == UIConstants.VALUE_ONE)
                        {
                            if (ObjCustomerSaveRequest.HierarchyMapDetails.User2UserID > 0)
                            {
                                //Send sms to customer(se mapping)
                                SendSeMappingSmsToCustomer(seName, seMob, customerMobile);
                                //Send sms to SE
                                SendSmsToSeOnMapping(customerName, seMob, customerMobile);
                            }
                            else
                            {
                                //Send email to asm(only asm mapping)
                                SendMappingEmailToASM(asmName, asmEmail);
                            }
                            objCustomerSaveResponse.ReturnValue = 1; //success
                        }
                        else
                            objCustomerSaveResponse.ReturnValue = 0; //Fail
                    }
                    else
                        objCustomerSaveResponse.ReturnValue = 0; //Fail
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + ": SaveAsmSeMappingDetails : " + ex.Message);
            }
            return objCustomerSaveResponse;
        }

        public AreaRetrieveResponse SaveAreaDetails(AreaRetrieveRequest objAreaRetrieveRequest)
        {
            AreaRetrieveResponse objAreaRetrieveResponse = null;
            try
            {
                SeMobileAppBO objSeMobileAppBO = new SeMobileAppBO();
                objAreaRetrieveResponse = objSeMobileAppBO.SaveAreaDetails(objAreaRetrieveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + ": SaveAreaDetails : " + ex.Message);
            }
            return objAreaRetrieveResponse;
        }

        public CatalogueRetriveResponse GetCatalogueDetails(CatalogueRetriveRequest ObjCatalogueRetriveRequest)
        {
            CatalogueRetriveResponse ObjCatalogueRetriveResponse = new CatalogueRetriveResponse();
            try
            {
                SeMobileAppBO objSeMobileAppBO = new SeMobileAppBO();
                ObjCatalogueRetriveResponse = objSeMobileAppBO.GetCatalogueDetails(ObjCatalogueRetriveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + ": GetCatalogueDetails : " + ex.Message);
            }
            return ObjCatalogueRetriveResponse;
        }

        #region EMail & SMS
        //SMS to customer on SE mapping
        public void SendSeMappingSmsToCustomer(string seName, string seMob, string CustMob)
        {
            try
            {
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse_SMS = null;
                MobileAppBO mobileAppBO = new MobileAppBO();
                //Set Template Getting Request
                SmsEmail ObjSmsEmail = new SmsEmail();
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.CustomerMappingNotificationsAsm);
                ObjSmsEmailRetrieveRequest.UserName = MERCHANT_USERNAME;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = CustMob;

                //For SMS
                ObjSmsEmailRetrieveRequest.Type = UIConstants.SMS;
                ObjSmsEmailRetrieveResponse_SMS = mobileAppBO.GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);

                for (int i = 0; i < ObjSmsEmailRetrieveResponse_SMS.lstSmsEmailDetails.Count; i++)
                {
                    //Dynamic template for SMS
                    List<AlertTemplateDynamicContent> DynamicTemplateSMS = new List<AlertTemplateDynamicContent>()
                                {
                                      new AlertTemplateDynamicContent("[Executive Name]", seName),
                                       new AlertTemplateDynamicContent("[Mobile Number]", seMob)
                                };
                    //For SMS
                    if (!string.IsNullOrEmpty(CustMob))
                    {
                        AlertUtiltityParameters alertUtiltityParametersSMS = new AlertUtiltityParameters
                                      (ObjSmsEmailRetrieveResponse_SMS.lstSmsEmailDetails[i].Type, CustMob, DynamicTemplateSMS, ObjSmsEmailRetrieveResponse_SMS.lstSmsEmailDetails[i]);
                        alertUtiltityParametersSMS.IsGetRequest = true;
                        var SMSResult = SendAlertUtility.SendAlert(alertUtiltityParametersSMS);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " :: SendSeMappingSmsToCustomer() :: " + ex.Message);
            }
        }

        //SMS to SE on SE mapping
        public void SendSmsToSeOnMapping(string Customername, string seMob, string CustMob)
        {
            try
            {
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse_SMS = null;
                MobileAppBO mobileAppBO = new MobileAppBO();
                //Set Template Getting Request
                SmsEmail ObjSmsEmail = new SmsEmail();
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.CustomerMapinningAlertForSE);
                ObjSmsEmailRetrieveRequest.UserName = MERCHANT_USERNAME;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = seMob;

                //For SMS
                ObjSmsEmailRetrieveRequest.Type = UIConstants.SMS;
                ObjSmsEmailRetrieveResponse_SMS = mobileAppBO.GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);
                for (int i = 0; i < ObjSmsEmailRetrieveResponse_SMS.lstSmsEmailDetails.Count; i++)
                {

                    //Dynamic template for SMS
                    List<AlertTemplateDynamicContent> DynamicTemplateSMS = new List<AlertTemplateDynamicContent>()
                                {
                                      new AlertTemplateDynamicContent("[Name]", Customername),
                                       new AlertTemplateDynamicContent("[Mobile Number]", CustMob)
                                };
                    //For SMS
                    if (!string.IsNullOrEmpty(seMob))
                    {
                        AlertUtiltityParameters alertUtiltityParametersSMS = new AlertUtiltityParameters
                                      (ObjSmsEmailRetrieveResponse_SMS.lstSmsEmailDetails[i].Type, seMob, DynamicTemplateSMS, ObjSmsEmailRetrieveResponse_SMS.lstSmsEmailDetails[i]);
                        alertUtiltityParametersSMS.IsGetRequest = true;
                        var SMSResult = SendAlertUtility.SendAlert(alertUtiltityParametersSMS);

                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " :: SendSeMappingSmsToCustomer() :: " + ex.Message);
            }
        }

        //Email to ASM on ASM mapping
        public void SendMappingEmailToASM(string asmName, string asmEmail)
        {
            try
            {
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse_Email = null;
                MobileAppBO mobileAppBO = new MobileAppBO();
                SmsEmail ObjSmsEmail = new SmsEmail();
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.CustomerMappingNotificationsAsm);
                ObjSmsEmailRetrieveRequest.UserName = MERCHANT_USERNAME;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = asmEmail;

                ObjSmsEmailRetrieveRequest.Type = UIConstants.EMAIL;
                ObjSmsEmailRetrieveResponse_Email = mobileAppBO.GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);

                if (ObjSmsEmailRetrieveResponse_Email.lstSmsEmailDetails != null)
                {
                    if (ObjSmsEmailRetrieveResponse_Email.lstSmsEmailDetails.Count > 0)
                    {
                        ObjSmsEmailRetrieveResponse_Email.lstSmsEmailDetails[0].Subject = ObjSmsEmailRetrieveResponse_Email.lstSmsEmailDetails[0].Subject + " " + DateTime.Now.ToString("dd/MM/yyyy");
                    }
                }
                for (int i = 0; i < ObjSmsEmailRetrieveResponse_Email.lstSmsEmailDetails.Count; i++)
                {
                    List<AlertTemplateDynamicContent> DynamicTemplateEmail = new List<AlertTemplateDynamicContent>()
                                {
                                      new AlertTemplateDynamicContent("[ASM Name]", asmName)
                                };
                    //For Email
                    if (!string.IsNullOrEmpty(asmEmail))
                    {
                        AlertUtiltityParameters alertUtiltityParameterEmail = new AlertUtiltityParameters
                                      (UIConstants.EMAIL, asmEmail, DynamicTemplateEmail, ObjSmsEmailRetrieveResponse_Email.lstSmsEmailDetails[i]);
                        alertUtiltityParameterEmail.MailHeaderText = UIConstants.OTHERS_HEADER;
                        var MailResult = SendAlertUtility.SendAlert(alertUtiltityParameterEmail);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "SendMappingEmailToASM()" + ex.Message);
            }
        }

        //SMS for login credential on successfull enrollment
        private void SendCustomerCredentialSMS(string CustomerName, string Password, string Mobile, string LoyaltyId)
        {
            try
            {
                MobileAppBO mobileAppBO = new MobileAppBO();
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                if (DOMAIN == UIConstants.WAVIN)
                {
                    ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.LoginCreds_UserReqst_ONL);
                }
                else
                {
                    ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.MembershipLoginCredentials);
                }
                ObjSmsEmailRetrieveRequest.Type = UIConstants.SMS;
                ObjSmsEmailRetrieveRequest.UserName = MERCHANT_USERNAME;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = Mobile;

                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse = mobileAppBO.GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);

                for (int i = 0; i < ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count; i++)
                {
                    if (DOMAIN == UIConstants.WAVIN)
                    {
                        if (ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i].Type == "SMS")
                        {
                            List<AlertTemplateDynamicContent> DynamicTemplate = new List<AlertTemplateDynamicContent>()
                                {
                                    new AlertTemplateDynamicContent("[Username]", LoyaltyId.ToString())
                                };
                            //Sending SMS
                            AlertUtiltityParameters alertUtiltityParametersSMS = new AlertUtiltityParameters
                                          (ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i].Type, Mobile, DynamicTemplate, ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i]);

                            alertUtiltityParametersSMS.IsGetRequest = true;
                            var SMSResult = SendAlertUtility.SendAlert(alertUtiltityParametersSMS);
                        }
                    }
                    else
                    {
                        List<AlertTemplateDynamicContent> DynamicTemplateSMS = new List<AlertTemplateDynamicContent>()
                                    {
                                      new AlertTemplateDynamicContent("[Client name]", CustomerName),
                                      new AlertTemplateDynamicContent("[membership id]", LoyaltyId),
                                      new AlertTemplateDynamicContent("[Pin]", Password),
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
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " :: SendCustomerCredentialSMS() :: " + ex.Message);
            }
        }

        //SMS for Sending Miss call number on successfull enrollment
        private void SendMissCallNumberSMS(string Mobile)
        {
            try
            {
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.MissedCallNotification);
                ObjSmsEmailRetrieveRequest.Type = UIConstants.SMS;
                ObjSmsEmailRetrieveRequest.UserName = MERCHANT_USERNAME;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = Mobile;
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse = new MobileAppBO().GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);

                for (int i = 0; i < ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count; i++)
                {
                    List<AlertTemplateDynamicContent> DynamicTemplateSMS = new List<AlertTemplateDynamicContent>() { };
                    AlertUtiltityParameters alertUtiltityParametersSMS = new AlertUtiltityParameters
                                  (ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i].Type, Mobile, DynamicTemplateSMS, ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i]);

                    alertUtiltityParametersSMS.IsGetRequest = true;
                    var SMSResult = SendAlertUtility.SendAlert(alertUtiltityParametersSMS);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " :: SendMissCallNumberSMS() :: " + ex.Message);
            }
        }

        public bool ForgotPasswordSEMobileApp(string UserName)
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
                UserLoginRetrieveResponse userRetrieveResponse = CheckIsAuthenticatedSEMobileApp(userRetrieveRequest);
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
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " ForgotPasswordSEMobileApp() " + ex.Message);
            }
            return result;
        }

        private void SendForgotPIN(string CustName, string Mobile, string PinNumber, string UsTyp, string Uname)
        {
            try
            {
                SeMobileAppBO ObjSEMobileAppBO = new SeMobileAppBO();
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.ForgotPasswordPIN);
                ObjSmsEmailRetrieveRequest.Type = UIConstants.SMS;
                ObjSmsEmailRetrieveRequest.UserName = Uname;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = Mobile;
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse = ObjSEMobileAppBO.GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);
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
                SeMobileAppBO ObjSEMobileAppBO = new SeMobileAppBO();
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.ForgotPasswordPIN);
                ObjSmsEmailRetrieveRequest.Type = UIConstants.EMAIL;
                ObjSmsEmailRetrieveRequest.UserName = Uname;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = Mail;
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse = ObjSEMobileAppBO.GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);
                if ((ObjSmsEmailRetrieveResponse != null) && (ObjSmsEmailRetrieveResponse.lstSmsEmailDetails != null) && (ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count > 0))
                {
                    for (int i = 0; i < ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count; i++)
                    {
                        List<AlertTemplateDynamicContent> DynamicTemplate = new List<AlertTemplateDynamicContent>()
                            {
                            new AlertTemplateDynamicContent(TO_NAME, CustName),
                            new AlertTemplateDynamicContent(PIN, PinNumber)
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
        #endregion EMail & SMS

        #region Is_Authenticate
        public UserLoginRetrieveResponse CheckIsAuthenticatedSEMobileApp(UserRetrieveRequest_Loyalty userRetrieveRequest)
        {
            SeMobileAppBO ObjSEMobileAppBO = new SeMobileAppBO();
            UserLoginRetrieveResponse objUserLoginRetrieveResponse = null;
            try
            {
                if (userRetrieveRequest.ActionType == UPDATE_CHANGED_PASSWORD)
                {
                    userRetrieveRequest.UserId = -1;
                    userRetrieveRequest.Password = Security.EncryptPassword(userRetrieveRequest.Password);
                    userRetrieveRequest.UserType = UIConstants.CUSTOMER_TYPE;
                }
                objUserLoginRetrieveResponse = ObjSEMobileAppBO.CheckIsAuthenticatedSEMobileApp(userRetrieveRequest);
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
                        ObjSEMobileAppBO.CheckIsAuthenticatedSEMobileApp(userRetrieveRequest);
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
        #endregion

        public CustomerRetrieveResponse GetDelerMapCustomerDetails(CustomerRetrieveRequest ObjCustomerRetrieveRequest)
        {
            CustomerRetrieveResponse objCustomerRetrieveResponse = null;
            try
            {
                SeMobileAppBO objSeMobileAppBO = new SeMobileAppBO();
                objCustomerRetrieveResponse = objSeMobileAppBO.GetDelerMapCustomerDetails(ObjCustomerRetrieveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "GetDelerMapCustomerDetails : " + ex.Message);
            }
            return objCustomerRetrieveResponse;
        }


        public LoyaltyUserReatrieveResponse GetPurchaseStockDetails(LoyaltyUserRetriveRequest objLoyaltyUserRetriveRequest)
        {
            LoyaltyUserReatrieveResponse objLoyaltyUserReatrieveResponse = null;
            try
            {
                SeMobileAppBO objSeMobileAppBO = new SeMobileAppBO();
                objLoyaltyUserReatrieveResponse = objSeMobileAppBO.GetPurchaseStockDetails(objLoyaltyUserRetriveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "GetPurchaseStockDetails : " + ex.Message);
            }
            return objLoyaltyUserReatrieveResponse;
        }

        public DreamGiftSaveResponse SaveDreamGiftDetails(DreamGiftSaveRequest objDreamGiftSaveRequest)
        {
            DreamGiftSaveResponse objDreamGiftSaveResponse = null;
            try
            {
                SeMobileAppBO objSeMobileAppBO = new SeMobileAppBO();
                if (!string.IsNullOrEmpty(objDreamGiftSaveRequest.JDesiredDate) && (objDreamGiftSaveRequest.JDesiredDate) != Convert.ToString("1/1/0001"))
                    objDreamGiftSaveRequest.DesiredDate = Convert.ToDateTime(objDreamGiftSaveRequest.JDesiredDate);
                objDreamGiftSaveResponse = objSeMobileAppBO.SaveDreamGiftDetails(objDreamGiftSaveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " SaveDreamGiftDetails() " + ex.Message);
            }
            return objDreamGiftSaveResponse;
        }

        public DreamGiftRetriveResponse GetCustomerDreamGiftList(DreamGiftRetriveRequest objDreamGiftRetriveRequest)
        {
            DreamGiftRetriveResponse objDreamGiftRetriveResponse = null;
            try
            {
                SeMobileAppBO objSeMobileAppBO = new SeMobileAppBO();
                objDreamGiftRetriveResponse = objSeMobileAppBO.GetCustomerDreamGiftList(objDreamGiftRetriveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetCustomerDreamGiftList() " + ex.Message);
            }
            return objDreamGiftRetriveResponse;
        }

        public ScrachCodeApprovalResponse GetScratchCodeApprovalRequest(ScrachCodeApprovalRequest objScrachCodeApprovalRequest)
        {
            ScrachCodeApprovalResponse objScrachCodeApprovalResponse = null;
            try
            {
                SeMobileAppBO objSeMobileAppBO = new SeMobileAppBO();
                objScrachCodeApprovalResponse = objSeMobileAppBO.GetScratchCodeApprovalRequest(objScrachCodeApprovalRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetScratchCodeApprovalRequest() " + ex.Message);
            }
            return objScrachCodeApprovalResponse;
        }

        public ScrachCodeApprovalResponse SavaeQRCodeApprovalStatus(ScrachCodeApprovalRequest ObjScrachCodeApprovalRequest)
        {
            ScrachCodeApprovalResponse objScrachCodeApprovalResponse = null;
            string ReturnMessage = string.Empty;
            try
            {
                SeMobileAppBO objSeMobileAppBO = new SeMobileAppBO();
                objScrachCodeApprovalResponse = objSeMobileAppBO.SavaeQRCodeApprovalStatus(ObjScrachCodeApprovalRequest);
                if (objScrachCodeApprovalResponse != null)
                {
                    ReturnMessage = objScrachCodeApprovalResponse.ReturnMessage;
                    if (ReturnMessage.Split('~')[0] == UIConstants.VALUE_ONE)
                    {
                        if (ObjScrachCodeApprovalRequest.QRCodeApprovalStatusID == -1)//-1 is rejected qr code
                        {
                            SendQRCodeRejectAlertSMS(ObjScrachCodeApprovalRequest.lstScrachCodeApproval[0].MemberMobile, Convert.ToString(ObjScrachCodeApprovalRequest.CodeCount));
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " SavaeQRCodeApprovalStatus() " + ex.Message);
            }
            return objScrachCodeApprovalResponse;
        }

        //public CustomerrPointsTrendResponse GetCustomerrPointsTrend(CustomerrPointsTrendRequest objCustomerrPointsTrendRequest)
        //{
        //    CustomerrPointsTrendResponse objCustomerrPointsTrendResponse = null;
        //    try
        //    {
        //        SeMobileAppBO objSeMobileAppBO = new SeMobileAppBO();
        //        objCustomerrPointsTrendResponse = objSeMobileAppBO.GetCustomerrPointsTrend(objCustomerrPointsTrendRequest);
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " SavaeQRCodeApprovalStatus() " + ex.Message);
        //    }
        //    return objCustomerrPointsTrendResponse;
        //}

        public Byte[] GetCustomerrPointsTrend(CustomerrPointsTrendRequest objCustomerrPointsTrendRequest)
        {
            Byte[] bytes = null;
            CustomerrPointsTrendResponse objCustomerrPointsTrendResponse = null;
            try
            {
                SeMobileAppBO objSeMobileAppBO = new SeMobileAppBO();
                objCustomerrPointsTrendResponse = objSeMobileAppBO.GetCustomerrPointsTrend(objCustomerrPointsTrendRequest);


                if (objCustomerrPointsTrendResponse.LstCustomerrPointsTrend != null && objCustomerrPointsTrendResponse.LstCustomerrPointsTrend.Count > 0)
                {
                    ReportViewer rdlcCustomerList = new ReportViewer();
                    rdlcCustomerList.LocalReport.DataSources.Clear();

                    rdlcCustomerList.LocalReport.ReportPath = rdlcReportPath;
                    rdlcCustomerList.LocalReport.EnableHyperlinks = true;
                    rdlcCustomerList.LocalReport.Refresh();
                    ReportDataSource TableDataSource = new ReportDataSource(ExportTableName, objCustomerrPointsTrendResponse.LstCustomerrPointsTrend);
                    rdlcCustomerList.LocalReport.DataSources.Add(TableDataSource);

                    //Export
                    Warning[] warnings;
                    string[] streamIds;
                    string mimeType = string.Empty;
                    string encoding = string.Empty;
                    string extension = string.Empty;
                    string fileName = "UserList" + "_" + System.DateTime.Now.ToString("ddMMyyyy_hhmm");
                    bytes = rdlcCustomerList.LocalReport.Render("pdf", null, out mimeType, out encoding, out extension, out streamIds, out warnings);
                    //Now that you have all the bytes representing the PDF report, buffer it and send it to the client.
                }
            }
            catch (Exception ex)
            {


                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetCustomerrPointsTrend() " + ex.Message);
            }
            return bytes;
        }

        private void SendQRCodeRejectAlertSMS(string CustMobile, string CodeCount)
        {
            try
            {
                MobileAppBO mobileAppBO = new MobileAppBO();
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();

                ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.QRCodeRejectAlert);
                ObjSmsEmailRetrieveRequest.Type = UIConstants.SMS;
                ObjSmsEmailRetrieveRequest.UserName = MERCHANT_USERNAME;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = CustMobile;
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse = mobileAppBO.GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);

                if ((ObjSmsEmailRetrieveResponse != null) && (ObjSmsEmailRetrieveResponse.lstSmsEmailDetails != null) && (ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count > 0))
                {
                    for (int i = 0; i < ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count; i++)
                    {
                        List<AlertTemplateDynamicContent> DynamicTemplate = new List<AlertTemplateDynamicContent>()
                            {
                            new AlertTemplateDynamicContent("[Code Count]", CodeCount)

                            };
                        //Sending SMS
                        AlertUtiltityParameters alertUtiltityParametersSMS = new AlertUtiltityParameters
                                      (ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i].Type, CustMobile, DynamicTemplate, ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i]);

                        alertUtiltityParametersSMS.IsGetRequest = true;
                        var Result = SendAlertUtility.SendAlert(alertUtiltityParametersSMS);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(SERVICE_CLASS_NAME + "::SendQRCodeRejectAlertSMS()::" + ex.Message);
            }
        }
        public TransactionApprovalRetrieveResponse GetPurchaseRequestDetailsForSE(TransactionApprovalRetrieveRequest ObjTransactionApprovalRetrieveRequest)
        {
            TransactionApprovalRetrieveResponse objGetPurchaseRequestDetailsList = null;
            try
            {
                SeMobileAppBO objSeMobileAppBO = new SeMobileAppBO();
                if (!string.IsNullOrEmpty(ObjTransactionApprovalRetrieveRequest.JFromDate) && (ObjTransactionApprovalRetrieveRequest.JFromDate != Convert.ToString("1/1/0001")))
                    ObjTransactionApprovalRetrieveRequest.FromDate = Convert.ToDateTime(ObjTransactionApprovalRetrieveRequest.JFromDate);
                if (!string.IsNullOrEmpty(ObjTransactionApprovalRetrieveRequest.JToDate) && (ObjTransactionApprovalRetrieveRequest.JToDate != Convert.ToString("1/1/0001")))
                    ObjTransactionApprovalRetrieveRequest.ToDate = Convert.ToDateTime(ObjTransactionApprovalRetrieveRequest.JToDate);
                objGetPurchaseRequestDetailsList = objSeMobileAppBO.GetPurchaseRequestDetailsForSE(ObjTransactionApprovalRetrieveRequest);

            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetPurchaseRequestDetailsForSE() " + ex.Message);
            }
            return objGetPurchaseRequestDetailsList;
        }

        private void sendCustomerWelcomeSMS(string CustomerName, string Mobile)
        {
            try
            {
                SmsEmail ObjSmsEmail = new SmsEmail();
                MobileAppBO objCommonAL = new MobileAppBO();
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.WelcomeAccountactivation);
                ObjSmsEmailRetrieveRequest.Type = UIConstants.SMS;
                ObjSmsEmailRetrieveRequest.UserName = MERCHANT_USERNAME;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = Mobile;
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse = objCommonAL.GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);
                for (int i = 0; i < ObjSmsEmailRetrieveResponse.lstSmsEmailDetails.Count; i++)
                {
                    List<AlertTemplateDynamicContent> DynamicTemplate = new List<AlertTemplateDynamicContent>(){
                new AlertTemplateDynamicContent ("[Client name]",CustomerName)};

                    AlertUtiltityParameters alertUtiltityParameters = new AlertUtiltityParameters
                   (ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i].Type, Mobile, DynamicTemplate, ObjSmsEmailRetrieveResponse.lstSmsEmailDetails[i]);
                    alertUtiltityParameters.IsGetRequest = true;

                    var Result = SendAlertUtility.SendAlert(alertUtiltityParameters);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(SERVICE_CLASS_NAME + " sendCustomerWelcomeSMS() : " + ex.Message);
            }
        }

        public WorkSiteInfoResponse SaveWorkSiteInfo(WorkSiteInfoRequest objWorkSiteInfoRequest)
        {
            WorkSiteInfoResponse objWorkSiteInfoResponse = null;
            SeMobileAppBO objSeMobileAppBO = new SeMobileAppBO();
            try
            {
                string FullPath = string.Empty, FileName = string.Empty;
                FullPath = HTTP_UPLOAD_FOLDER_PATH + "~/UploadFiles/WorkSiteImage/";
                if (objWorkSiteInfoRequest.SiteImageURl != null)
                {
                    FileName = DateTime.Now.ToFileTime().ToString() + ".png";
                    System.Drawing.Image SiteImageURl = UICommon.Base64ToImageConverter(objWorkSiteInfoRequest.SiteImageURl);

                    SiteImageURl.Save((FullPath.Replace("~", "") + FileName), ImageFormat.Png);
                    objWorkSiteInfoRequest.SiteImageURl = FileName;

                }
                objWorkSiteInfoRequest.Domain = DOMAIN;
                objWorkSiteInfoResponse = objSeMobileAppBO.SaveWorkSiteInfo(objWorkSiteInfoRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + ":: SaveWorkSiteInfo ::" + ex.Message);
            }
            return objWorkSiteInfoResponse;
        }
        public WorkSiteInfoResponse GetWorkSiteDetails(WorkSiteInfoRequest objWorkSiteInfoRequest)
        {
            WorkSiteInfoResponse objWorkSiteInfoResponse = null;
            try
            {
                SeMobileAppBO objSeMobileAppBO = new SeMobileAppBO();
                objWorkSiteInfoResponse = objSeMobileAppBO.GetWorkSiteDetails(objWorkSiteInfoRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + ":: GetWorkSiteDetails ::" + ex.Message);
            }
            return objWorkSiteInfoResponse;
        }
        public AreaRetrieveResponse GetAreaDetails(AreaRetrieveRequest objAreaRetrieveRequest)
        {
            AreaRetrieveResponse objAreaRetrieveResponse = null;
            try
            {
                SeMobileAppBO objSeMobileAppBO = new SeMobileAppBO();
                objAreaRetrieveResponse = objSeMobileAppBO.GetAreaDetails(objAreaRetrieveRequest);
            }
            catch (Exception ex)
            {

                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " : GetAreaDetails() : " + ex.Message);
            }
            return objAreaRetrieveResponse;
        }
        public AttributesRetrieveResponse GetAttributeDetails(AttributesRetrieveRequest objAttributesRetrieveRequest)
        {
            AttributesRetrieveResponse objAttributesRetrieveResponse = null;
            try
            {
                SeMobileAppBO objSeMobileAppBO = new SeMobileAppBO();
                objAttributesRetrieveResponse = objSeMobileAppBO.GetAttributeDetails(objAttributesRetrieveRequest);
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
        public LookUpRetrieveResponse GetLookUpDetails(LookUpRetrieveRequest objLookUpRetrieveRequest)
        {
            LookUpRetrieveResponse objLookUpRetrieveResponse = null;
            try
            {
                SeMobileAppBO objSeMobileAppBO = new SeMobileAppBO();
                objLookUpRetrieveResponse = objSeMobileAppBO.GetLookUpDetails(objLookUpRetrieveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "GetLookUpDetails : " + ex.Message);
            }
            finally
            {
            }
            return objLookUpRetrieveResponse;
        }
        public VendorRetrieveResponse GetVendorDetails(VendorRetrieveRequest ObjVendorRetrieveRequest)
        {
            VendorRetrieveResponse ObjVendorRetrieveResponse = null;
            try
            {
                SeMobileAppBO objSeMobileAppBO = new SeMobileAppBO();
                ObjVendorRetrieveResponse = objSeMobileAppBO.GetVendorDetails(ObjVendorRetrieveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "GetVendorDetails() : " + ex.Message);
            }
            return ObjVendorRetrieveResponse;
        }
        public RedemptionPlannerRetriveResponse GetPlannerAddedOrNot(RedemptionPlannerRetriveRequest objPlannerRetriveRequest)
        {
            RedemptionPlannerRetriveResponse objRedemptionPlannerRetriveResponse = null;
            try
            {
                SeMobileAppBO objSeMobileAppBO = new SeMobileAppBO();
                objRedemptionPlannerRetriveResponse = objSeMobileAppBO.GetPlannerAddedOrNot(objPlannerRetriveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetPlannerAddedOrNot() " + ex.Message);
            }
            return objRedemptionPlannerRetriveResponse;
        }
        public QueryInfoByHelpTopicResponse GetQueryResponseInformation(QueryInfoByHelpTopicRequest objQueryInfoByHelpTopicRequest)
        {
            QueryInfoByHelpTopicResponse objQueryInfoByHelpTopicResponse = new QueryInfoByHelpTopicResponse();
            try
            {
                SeMobileAppBO objSeMobileAppBO = new SeMobileAppBO();
                objQueryInfoByHelpTopicResponse = objSeMobileAppBO.GetQueryResponseInformation(objQueryInfoByHelpTopicRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "GetQueryResponseInformation () :: " + ex.Message);
            }
            return objQueryInfoByHelpTopicResponse;
        }
        public HelpTopicRetrieveResponse GetHelpTopics(HelpTopicRetrieveRequest objHelpTopicRetrieveRequest, string ToTranslateLanguage = "")
        {
            GoogleTranslate google = new GoogleTranslate(GOOGLE_API_KEY);
            HelpTopicRetrieveResponse objHelpTopicRetrieveResponse = new HelpTopicRetrieveResponse();
            try
            {
                SeMobileAppBO objSeMobileAppBO = new SeMobileAppBO();
                objHelpTopicRetrieveResponse = objSeMobileAppBO.GetHelpTopics(objHelpTopicRetrieveRequest);


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

        public QueryReplyRetrieveResponse ReplyToCustomerQuery(QueryReplyRetrieveRequest objQueryReplyRetrieveRequest)
        {
            QueryReplyRetrieveResponse objQueryReplyRetrieveResponse = new QueryReplyRetrieveResponse();
            try
            {
                if (objQueryReplyRetrieveRequest.IsQueryFromMobile)
                {
                    string FullPath = string.Empty, FileName = string.Empty;
                    FullPath = HTTP_UPLOAD_FOLDER_PATH + PROFILE_PIC_FOLDER_PATH;
                    //Display Image
                    if (!string.IsNullOrEmpty(objQueryReplyRetrieveRequest.ImageUrl))
                    {
                        FileName = "CustomerQuery_" + objQueryReplyRetrieveRequest.LoyaltyID + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".png";
                        bool Result = UICommon.SaveBase64StringIntoFile(FullPath.Replace("~", ""), objQueryReplyRetrieveRequest.ImageUrl, FileName);
                        objQueryReplyRetrieveRequest.ImageUrl = PROFILE_PIC_FOLDER_PATH.Replace("\\", "/") + FileName;
                    }
                }
                SeMobileAppBO objSeMobileAppBO = new SeMobileAppBO();
                objQueryReplyRetrieveResponse = objSeMobileAppBO.ReplyToCustomerQuery(objQueryReplyRetrieveRequest);
                if (DOMAIN == UIConstants.MILLER_CLUB)
                {
                    string[] args = objQueryReplyRetrieveResponse.ReturnMessage.ToString().Split('~');
                    if (args[0] == UIConstants.VALUE_ONE)
                    {
                        SendSMSAndEmailNotificationForCustomer(UIConstants.SMS, objQueryReplyRetrieveRequest.LoyaltyID, objQueryReplyRetrieveRequest.CustomerMobile, objQueryReplyRetrieveRequest.SE_Name, objQueryReplyRetrieveRequest.CustomerName, objQueryReplyRetrieveRequest.QueryID, Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.QueryReplyAlertForCustomer));
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "ReplyToCustomerQuery () :: " + ex.Message);
            }
            return objQueryReplyRetrieveResponse;
        }

        private void SendSMSAndEmailNotificationForCustomer(string Type, string LoyaltyId, string MobileOrEmailID, string SE_Name, string CustomerName, string QueryNo, string Template)
        {
            try
            {
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                ObjSmsEmailRetrieveRequest.TemplateName = Template;
                ObjSmsEmailRetrieveRequest.UserName = LoyaltyId;
                ObjSmsEmailRetrieveRequest.Type = Type;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = MobileOrEmailID;
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse_SMS = new SeMobileAppBO().GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);

                if (ObjSmsEmailRetrieveResponse_SMS != null && ObjSmsEmailRetrieveResponse_SMS.lstSmsEmailDetails.Count > 0)
                {
                    List<AlertTemplateDynamicContent> DynamicTemplate = new List<AlertTemplateDynamicContent>()
                    {
                     new AlertTemplateDynamicContent("[User Name]", LoyaltyId),
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
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "SendSMSAndEmailNotificationForCustomer() : " + ex.Message);
            }
        }

        public GetSurveyQuestionAnswerResponse GetSurveyQuestionAnswer(GetSurveyQuestionAnswerRequest objGetSurveyQuestionAnswerRequest)
        {
            GetSurveyQuestionAnswerResponse objGetSurveyQuestionAnswerResponse = null;
            try
            {
                SeMobileAppBO objSeMobileAppBO = new SeMobileAppBO();
                objGetSurveyQuestionAnswerResponse = objSeMobileAppBO.GetSurveyQuestionAnswer(objGetSurveyQuestionAnswerRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + ":: GetSurveyQuestionAnswer() ::" + ex.Message);
            }
            return objGetSurveyQuestionAnswerResponse;
        }
        public SaveSurveyQuestionAnswerResponse SaveSurveyQuestionAnswer(SaveSurveyQuestionAnswerRequest objSaveSurveyQuestionAnswerRequest)
        {
            SaveSurveyQuestionAnswerResponse objGetSurveyQuestionAnswerResponce = null;
            try
            {
                SeMobileAppBO objSeMobileAppBO = new SeMobileAppBO();
                objGetSurveyQuestionAnswerResponce = objSeMobileAppBO.SaveSurveyQuestionAnswer(objSaveSurveyQuestionAnswerRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "SaveSurveyQuestionAnswer()" + ex.Message);
            }
            return objGetSurveyQuestionAnswerResponce;
        }

        public GetUserMappingRetriveResponse GetUserRegionMapping(GetUserMappingRetriveRequest objGetUserMappingRetriveRequest)
        {
            GetUserMappingRetriveResponse objGetUserMappingRetriveResponse = null;
            try
            {
                SeMobileAppBO objSeMobileAppBO = new SeMobileAppBO();
                objGetUserMappingRetriveResponse = objSeMobileAppBO.GetUserRegionMapping(objGetUserMappingRetriveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "GetUserRegionMapping()" + ex.Message);
            }
            return objGetUserMappingRetriveResponse;
        }

        public GetOrderDetailsResponseApi GetOrderDetails(GetOrderDetailsRequest objGetOrderDetailsRequest)
        {
            GetOrderDetailsResponseApi objGetOrderDetailsResponse = null;
            try
            {
                SeMobileAppBO objSeMobileAppBO = new SeMobileAppBO();
                objGetOrderDetailsResponse = objSeMobileAppBO.GetOrderDetails(objGetOrderDetailsRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "GetUserRegionMapping()" + ex.Message);
            }
            return objGetOrderDetailsResponse;
        }

        public GetOrderDetailsResponseApi ApproveOrderDetails(GetOrderDetailsRequestApi objGetOrderDetailsRequest)
        {
            GetOrderDetailsResponseApi objGetOrderDetailsResponse = null;
            try
            {
                SeMobileAppBO objSeMobileAppBO = new SeMobileAppBO();
                objGetOrderDetailsResponse = objSeMobileAppBO.ApproveOrderDetails(objGetOrderDetailsRequest);
                if (objGetOrderDetailsResponse != null)
                {
                    if (DOMAIN == UIConstants.MILLER_CLUB)
                    {
                        if (objGetOrderDetailsResponse.ReturnMessage == UIConstants.VALUE_ONE)
                        {
                            GetOrderDetailsResponseApi ObjGetOrderDetailsResponseOnOrder = null;
                            GetOrderDetailsRequest ObjGetOrderDetailsRequestOnOrder = new GetOrderDetailsRequest();
                            ObjGetOrderDetailsRequestOnOrder.ActionType = 9;
                            ObjGetOrderDetailsRequestOnOrder.OrderNumber = objGetOrderDetailsRequest.OrderMobNumber; //Mobile Number
                            ObjGetOrderDetailsResponseOnOrder = objSeMobileAppBO.GetOrderDetails(ObjGetOrderDetailsRequestOnOrder);
                            if (ObjGetOrderDetailsResponseOnOrder != null && ObjGetOrderDetailsResponseOnOrder.lstOrderDetails.Count > 0)
                            {
                                if (objGetOrderDetailsRequest.OrderMobStatus == Convert.ToInt32(UIConstants.VALUE_MINUS_ONE))
                                {
                                    SendApproveAndRejectSMSAndEmailToCustomer(UIConstants.EMAIL, objGetOrderDetailsRequest.LoyaltyId, objGetOrderDetailsRequest.CustomerEmailId, objGetOrderDetailsRequest.OrderMobNumber, objGetOrderDetailsRequest.Remarks, objGetOrderDetailsRequest.CustomerName, objGetOrderDetailsRequest.OrderDate, Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.OrderCancellation), ObjGetOrderDetailsResponseOnOrder.lstOrderDetails);
                                    SendApproveAndRejectSMSAndEmailToCustomer(UIConstants.SMS, objGetOrderDetailsRequest.LoyaltyId, objGetOrderDetailsRequest.CustomerMobileNo, objGetOrderDetailsRequest.OrderMobNumber, objGetOrderDetailsRequest.Remarks, objGetOrderDetailsRequest.CustomerName, objGetOrderDetailsRequest.OrderDate, Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.OrderCancellation), ObjGetOrderDetailsResponseOnOrder.lstOrderDetails);
                                }
                                else if (objGetOrderDetailsRequest.OrderMobStatus == Convert.ToInt32(UIConstants.VALUE_MINUS_ONE))
                                {
                                    SendApproveAndRejectSMSAndEmailToCustomer(UIConstants.EMAIL, objGetOrderDetailsRequest.LoyaltyId, objGetOrderDetailsRequest.CustomerEmailId, objGetOrderDetailsRequest.OrderMobNumber, objGetOrderDetailsRequest.Remarks, objGetOrderDetailsRequest.CustomerName, objGetOrderDetailsRequest.OrderDate, Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.OrderConfirmation), ObjGetOrderDetailsResponseOnOrder.lstOrderDetails);
                                    SendApproveAndRejectSMSAndEmailToCustomer(UIConstants.SMS, objGetOrderDetailsRequest.LoyaltyId, objGetOrderDetailsRequest.CustomerMobileNo, objGetOrderDetailsRequest.OrderMobNumber, objGetOrderDetailsRequest.Remarks, objGetOrderDetailsRequest.CustomerName, objGetOrderDetailsRequest.OrderDate, Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.OrderConfirmation), ObjGetOrderDetailsResponseOnOrder.lstOrderDetails);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "ApproveOrderDetails()" + ex.Message);
            }
            return objGetOrderDetailsResponse;
        }

        private void SendApproveAndRejectSMSAndEmailToCustomer(string Type, string LoyaltyId, string MobileOrEmail, string OrderNumber, string Remarks, string CustomerName, string OrderDate, string Template, List<OrderProductDetailsApi> lstCart)
        {
            try
            {
                SmsEmail ObjSmsEmail = new SmsEmail();
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();
                ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.OrderCancellation);
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
                             new AlertTemplateDynamicContent("[Remark]", Remarks),
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
                            new AlertTemplateDynamicContent("[Remark]",Remarks),
                            new AlertTemplateDynamicContent("[date]",OrderDate)
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
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "SendApproveAndRejectSMSAndEmailToCustomer : " + ex.Message);
            }
        }

        public UserLoginRetrieveResponse CheckIsAuthenticated(UserRetrieveRequest_Loyalty userRetrieveRequest)
        {
            UserLoginRetrieveResponse objUserLoginRetrieveResponse = null;
            try
            {
                userRetrieveRequest.DomainName = DOMAIN;
                objUserLoginRetrieveResponse = new SeMobileAppBO().CheckIsAuthenticated(userRetrieveRequest);
                if (objUserLoginRetrieveResponse == null)
                {
                    objUserLoginRetrieveResponse = new UserLoginRetrieveResponse();
                    objUserLoginRetrieveResponse.UserList = new List<User_Loyalty_Base>();
                    User_Loyalty_Base objUser_Loyalty_Base = new User_Loyalty_Base();
                    objUserLoginRetrieveResponse.UserList.Add(objUser_Loyalty_Base);
                    objUserLoginRetrieveResponse.UserList[0].Result = -1;
                }
                else if (userRetrieveRequest.ActionType == UPDATE_CHANGED_PASSWORD
                            || userRetrieveRequest.ActionType == UPDATE_CUSTOMER_PASSWORD
                            || userRetrieveRequest.ActionType == CHECK_IS_USER
                            || userRetrieveRequest.ActionType == MERCHANT_BY_LOCATION)
                {
                    objUserLoginRetrieveResponse.UserList[0].Result = 1;
                }
                else if (userRetrieveRequest.ActionType != CHECK_LOLALTYID_EXISTS)
                {
                    if (objUserLoginRetrieveResponse != null && objUserLoginRetrieveResponse.UserList != null && objUserLoginRetrieveResponse.UserList.Count > 0)
                    {
                        if (objUserLoginRetrieveResponse.UserList[0].Password != null && (objUserLoginRetrieveResponse.UserList[0].Result != 6))
                        {
                            if (Security.ValidateUser(userRetrieveRequest.Password, objUserLoginRetrieveResponse.UserList[0].Password))
                            {
                                if (!string.IsNullOrEmpty(userRetrieveRequest.PushID))
                                {
                                    userRetrieveRequest.ActionType = "SaveLogDetails";
                                    new SeMobileAppBO().CheckIsAuthenticated(userRetrieveRequest);
                                }
                                objUserLoginRetrieveResponse.UserList[0].Result = 1;
                            }
                            else
                            {
                                objUserLoginRetrieveResponse.UserList[0].Result = -1;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "CheckIsAuthenticated : " + ex.Message);
            }
            return objUserLoginRetrieveResponse;
        }

        public CustomerDashboardRetrieveResponse GetDashboardDetailsSEApp(CustomerDashboardRetrieveRequest ObjCustomerDashboardRetrieveReq)
        {
            CustomerDashboardRetrieveResponse objCustomerDashboardRetrieveResponse = null;
            try
            {
                objCustomerDashboardRetrieveResponse = new SeMobileAppBO().GetDashboardDetailsSEApp(ObjCustomerDashboardRetrieveReq);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "GetUserRegionMapping()" + ex.Message);
            }
            return objCustomerDashboardRetrieveResponse;
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
                {
                    objSaveOrderRequest.ReceiptImage = UIConstants.VALUE_EMPTY;
                }
                if (!string.IsNullOrEmpty(objSaveOrderRequest.ChequeImage))
                {
                    FullPath = HTTP_UPLOAD_FOLDER_PATH + RECEIPT_IMAGE_FOLDER_PATH;
                    FileName = objSaveOrderRequest.LoyaltyId + "_ChequeImage" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".png";
                    bool Result = UICommon.SaveBase64StringIntoFile(FullPath.Replace("~", ""), objSaveOrderRequest.ChequeImage, FileName);
                    objSaveOrderRequest.ChequeImage = FileName;
                }
                else
                    objSaveOrderRequest.ChequeImage = UIConstants.VALUE_EMPTY;

                objSaveOrderResponse = new SeMobileAppBO().SavePaymentReceiptDetails(objSaveOrderRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "SavePaymentReceiptDetails()" + ex.Message);
            }
            return objSaveOrderResponse;
        }

        public QueryCenterRetrieveResponse GetCustomerQueriesDetails(QueryCenterRetrieveRequest objQueryCenterRetrieveRequest)
        {
            QueryCenterRetrieveResponse objQueryCenterRetrieveResponse = new QueryCenterRetrieveResponse();
            try
            {
                objQueryCenterRetrieveResponse = new SeMobileAppBO().GetCustomerQueriesDetails(objQueryCenterRetrieveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + ":: GetCustomerQueriesDetails() :: " + ex.Message);
            }
            return objQueryCenterRetrieveResponse;
        }

        public QueryInfoByHelpTopicResponse GetCustomerQueryInfo(QueryInfoByHelpTopicRequest objQueryInfoByHelpTopicRequest)
        {
            QueryInfoByHelpTopicResponse objQueryInfoByHelpTopicResponse = new QueryInfoByHelpTopicResponse();
            try
            {
                objQueryInfoByHelpTopicResponse = new SeMobileAppBO().GetCustomerQueryInfo(objQueryInfoByHelpTopicRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "GetCustomerQueryInfo () :: " + ex.Message);
            }
            return objQueryInfoByHelpTopicResponse;
        }

        public DeliveryManagementDetailsResponse GetDriverCurrentLocationDetails(DeliveryManagementDetailsRequest objDeliveryManagementDetailsRequest)
        {
            DeliveryManagementDetailsResponse objDeliveryManagementDetailsResponse = new DeliveryManagementDetailsResponse();
            try
            {
                objDeliveryManagementDetailsResponse = new SeMobileAppBO().GetDriverCurrentLocationDetails(objDeliveryManagementDetailsRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "GetDriverCurrentLocationDetails () :: " + ex.Message);
            }
            return objDeliveryManagementDetailsResponse;
        }

        public UserSummaryResponse GetUserSummaryDetails(UserSummaryRequest objUserSummaryRequest)
        {
            UserSummaryResponse objUserSummaryResponse = new UserSummaryResponse();
            try
            {
                objUserSummaryRequest.Domain = DOMAIN;
                objUserSummaryResponse = new SeMobileAppBO().GetUserSummaryDetails(objUserSummaryRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "GetDriverCurrentLocationDetails () :: " + ex.Message);
            }
            return objUserSummaryResponse;
        }

        public CustomerRetrieveResponseMobileApiJson GetCustomerDetailsForJson(CustomerRetrieveRequestMobileApiJson ObjCustomerRetrieveRequest)
        {
            CustomerRetrieveResponseMobileApiJson objCustomerRetrieveResponse = null;
            try
            {
                SeMobileAppBO objSeMobileAppBO = new SeMobileAppBO();
                ObjCustomerRetrieveRequest.Domain = DOMAIN;
                objCustomerRetrieveResponse = objSeMobileAppBO.GetCustomerDetailsForJson(ObjCustomerRetrieveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "GetCustomerDetailsForJson : " + ex.Message);
            }
            return objCustomerRetrieveResponse;
        }

        public UserProfileDetailsResponse GetUserProfileDetails(UserProfileDetailsRequest objUserProfileDetailsRequest)
        {
            UserProfileDetailsResponse objUserProfileDetailsResponse = new UserProfileDetailsResponse();
            try
            {
                objUserProfileDetailsResponse = new SeMobileAppBO().GetUserProfileDetails(objUserProfileDetailsRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "GetDriverCurrentLocationDetails () :: " + ex.Message);
            }
            return objUserProfileDetailsResponse;
        }

        public SaveFuelManagementResponse SaveFuelManagementDetails(SaveFuelManagementRequest objSaveFuelManagementRequest)
        {
            SaveFuelManagementResponse objSaveFuelManagementResponse = null;
            string FullPath, FileName = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(objSaveFuelManagementRequest.ReceiptImage))
                {
                    FullPath = HTTP_UPLOAD_FOLDER_PATH + RECEIPT_IMAGE_FOLDER_PATH;
                    FileName = objSaveFuelManagementRequest.FuelStationName + "_ReceiptImage" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".png";
                    bool Result = UICommon.SaveBase64StringIntoFile(FullPath.Replace("~", ""), objSaveFuelManagementRequest.ReceiptImage, FileName);
                    objSaveFuelManagementRequest.ReceiptImage = FileName;
                }
                objSaveFuelManagementResponse = new SeMobileAppBO().SaveFuelManagementDetails(objSaveFuelManagementRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "SaveFuelManagementDetails () :: " + ex.Message);
            }
            return objSaveFuelManagementResponse;
        }

        public GetFuelManagementResponse GetFuelManagementDetails(GetFuelManagementRequest objGetFuelManagementRequest)
        {
            GetFuelManagementResponse objGetFuelManagementResponse = new GetFuelManagementResponse();
            try
            {
                objGetFuelManagementResponse = new SeMobileAppBO().GetFuelManagementDetails(objGetFuelManagementRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "GetFuelManagementDetails () :: " + ex.Message);
            }
            return objGetFuelManagementResponse;
        }

        public GetDeliveryOrderDetailsResponse GetDeliveryOrderDetails(GetDeliveryOrderDetailsRequest objGetDeliveryOrderDetailsRequest)
        {
            GetDeliveryOrderDetailsResponse objGetDeliveryOrderDetailsResponse = new GetDeliveryOrderDetailsResponse();
            try
            {
                objGetDeliveryOrderDetailsResponse = new SeMobileAppBO().GetDeliveryOrderDetails(objGetDeliveryOrderDetailsRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "GetFuelManagementDetails () :: " + ex.Message);
            }
            return objGetDeliveryOrderDetailsResponse;
        }

        public ReferralConvRetriveResponse GetReferenceAndEntrollmentBonus(ReferralConvRetriveRequest objReferralConvRetriveRequest)
        {
            ReferralConvRetriveResponse objReferralConvRetriveResponse = null;
            try
            {
                objReferralConvRetriveRequest.Domain = DOMAIN;
                objReferralConvRetriveResponse = new SeMobileAppBO().GetReferenceAndEntrollmentBonus(objReferralConvRetriveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetReferenceAndEntrollmentBonus() " + ex.Message);
            }
            return objReferralConvRetriveResponse;
        }

        public QueryInfoByHelpTopicResponseJson GetCustomerQueriesDetailsJSON(QueryInfoByHelpTopicRequestJson objQueryInfoByHelpTopicRequestJson)
        {
            QueryInfoByHelpTopicResponseJson objQueryInfoByHelpTopicResponseJson = new QueryInfoByHelpTopicResponseJson();
            try
            {
                objQueryInfoByHelpTopicResponseJson = new SeMobileAppBO().GetCustomerQueriesDetailsJSON(objQueryInfoByHelpTopicRequestJson);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + ":: GetCustomerQueriesDetails() :: " + ex.Message);
            }
            return objQueryInfoByHelpTopicResponseJson;
        }

        public GetClaimApprovalStatusResponse GetClaimApprovalStatusDetails(GetClaimApprovalStatusRequest ObjGetClaimApprovalStatusRequest)
        {
            GetClaimApprovalStatusResponse ObjGetClaimApprovalStatusResponse = null;
            try
            {
                ObjGetClaimApprovalStatusResponse = new SeMobileAppBO().GetClaimApprovalStatusDetails(ObjGetClaimApprovalStatusRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetClaimApprovalStatusDetails() " + ex.Message);
            }
            return ObjGetClaimApprovalStatusResponse;
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
                objProductSaveResponse = new SeMobileAppBO().SaveClaimRequestAtomberg(objProductSaveRequest);

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

        public VoucherRedemptionRetriveResponse SaveVoucherRedemptionRequest(VoucherRedemptionRetriveRequest ObjVoucherRedemptionRetriveRequest)
        {
            VoucherRedemptionRetriveResponse objVoucherRedemptionRetriveResponse = new VoucherRedemptionRetriveResponse();
            CustomerRetrieveResponse ObjCustomerRetrieveResponse = new CustomerRetrieveResponse();
            try
            {
                objVoucherRedemptionRetriveResponse = new SeMobileAppBO().SaveVoucherRedemptionRequest(ObjVoucherRedemptionRetriveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + ":: SaveVoucherRedemptionRequest() :: " + ex.Message);
            }
            return objVoucherRedemptionRetriveResponse;
        }
        public CatalogueRetriveResponse GetVoucherRedemptionList(CatalogueRetriveRequest ObjCatalogueRetriveRequest)
        {
            CatalogueRetriveResponse objCatalogueRetriveResponse = new CatalogueRetriveResponse();

            try
            {
                objCatalogueRetriveResponse = new SeMobileAppBO().GetVoucherRedemptionList(ObjCatalogueRetriveRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + ":: GetVoucherRedemptionList() :: " + ex.Message);
            }
            return objCatalogueRetriveResponse;
        }
        public OfflineCodesResponse SaveQRCodeDetailsBulk(OfflineCodesRequest ObjOfflineCodesRequest)
        {

            OfflineCodesResponse ObjOfflineCodesResponse = new OfflineCodesResponse();
            try
            {
                ObjOfflineCodesRequest.Domain = DOMAIN;

                ObjOfflineCodesResponse = new SeMobileAppBO().SaveQRCodeDetailsBulk(ObjOfflineCodesRequest);
                if (DOMAIN != UIConstants.CENTURY_PLY)
                {
                    if (ObjOfflineCodesResponse.QRCodeSaveResponseList != null && ObjOfflineCodesResponse.QRCodeSaveResponseList.Count > 0)
                    {
                        if (ObjOfflineCodesResponse.QRCodeSaveResponseList[0].IsNotional == 1)
                        {
                            CustomerRetrieveRequest objCustomerRetrieveRequest = new CustomerRetrieveRequest();
                            objCustomerRetrieveRequest.ActionType = 1;
                            objCustomerRetrieveRequest.LoyaltyId = ObjOfflineCodesRequest.LoyaltyID;
                            CustomerRetrieveResponse objCustomerRetrieveResponse = new SeMobileAppBO().GetUserDetailsForPushNotification(objCustomerRetrieveRequest);
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
        private void SendGetUserDetailsForPushNotifications(string UserName, string Mobile, string FirstName, string CustomerType = "")
        {
            try
            {
                SmsEmailRetrieveResponse ObjSmsEmailRetrieveResponse = new SmsEmailRetrieveResponse();
                SmsEmail ObjSmsEmail = new SmsEmail();
                SmsEmailRetrieveRequest ObjSmsEmailRetrieveRequest = new SmsEmailRetrieveRequest();

                ObjSmsEmailRetrieveRequest.TemplateName = Convert.ToString(Enumeration_Loyalty.SmsEmailTemplates.ScannedCodeIsGettingDeviated);

                ObjSmsEmailRetrieveRequest.UserName = UserName;
                ObjSmsEmailRetrieveRequest.EmailOrMobile = Mobile;
                ObjSmsEmailRetrieveRequest.Type = "SMS";
                ObjSmsEmailRetrieveResponse = new SeMobileAppBO().GetEmailSmsTemplateMobileApp(ObjSmsEmailRetrieveRequest);
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
        public CustomerRetrieveResponse GetUserDetailsForPushNotification(CustomerRetrieveRequest objCustomerRetrieveRequest)
        {
            CustomerRetrieveResponse objCustomerRetrieveResponse = new CustomerRetrieveResponse();
            try
            {
                objCustomerRetrieveResponse = new SeMobileAppBO().GetUserDetailsForPushNotification(objCustomerRetrieveRequest);
            }
            catch (Exception ee)
            {

                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "GetUserDetailsForPushNotification() : " + ee.Message);
            }
            return new SeMobileAppBO().GetUserDetailsForPushNotification(objCustomerRetrieveRequest);
        }
        public CustomerRetrieveResponse GetCustomerDetailsOnPanStatus(Customer objGetCustomerDetailsOnPanStatus)
        {
            string SubstringOfDate = null;
            CustomerRetrieveResponse objCatalogueRetriveResponse = new CustomerRetrieveResponse();
            try
            {
                if (!string.IsNullOrEmpty(objGetCustomerDetailsOnPanStatus.FinancialDate))
                {
                    SubstringOfDate = objGetCustomerDetailsOnPanStatus.FinancialDate.Substring(0, 14);
                    SubstringOfDate = SubstringOfDate.Replace(UIConstants.VALUE_FIRST, UIConstants.VALUE_ONE);
                    objGetCustomerDetailsOnPanStatus.FromDate = Convert.ToDateTime(SubstringOfDate);
                }

                if (objGetCustomerDetailsOnPanStatus.PanStatusId == Convert.ToInt32(UIConstants.VALUE_ONE))
                {
                    objGetCustomerDetailsOnPanStatus.PanStatus = UIConstants.COMPLETED.ToString();

                }
                else if (objGetCustomerDetailsOnPanStatus.PanStatusId == Convert.ToInt32(UIConstants.VALUE_TWO))
                {
                    objGetCustomerDetailsOnPanStatus.PanStatus = UIConstants.INCOMPLETED.ToString();
                }
                objCatalogueRetriveResponse = new SeMobileAppBO().GetCustomerDetailsOnPanStatus(objGetCustomerDetailsOnPanStatus);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + ":: GetCustomerDetailsOnPanStatus() :: " + ex.Message);
            }
            return objCatalogueRetriveResponse;
        }
        public PanSeAttibutesResponse GetExternAttibutesDetails(string ActionType)
        {
            PanSeAttibutesResponse ObjPanSeAttibutesResponse = new PanSeAttibutesResponse();
            try
            {
                ObjPanSeAttibutesResponse = new SeMobileAppBO().GetExternAttibutesDetails(ActionType);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + ":: GetExternAttibutesDetails() :: " + ex.Message);
            }
            return ObjPanSeAttibutesResponse;
        }
        public PushHistoryResponce GetPushHistoryDetails(PushHistoryRequest objPushHistoryRequest)
        {
            PushHistoryResponce objPushHistoryResponce = null;
            try
            {
                objPushHistoryResponce = new SeMobileAppBO().GetPushHistoryDetails(objPushHistoryRequest);
            }
            catch (Exception ex)
            {

                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "GetPushHistoryDetails()" + ex.Message);
            }

            return objPushHistoryResponce;
        }

        public TransctionResponse GetCustomerTrxnDetails(TransctionRequest objTransctionRequest)
        {
            TransctionResponse objTransctionResponse = null;
            try
            {
                objTransctionResponse = new SeMobileAppBO().GetCustomerTrxnDetails(objTransctionRequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + "GetCustomerTrxnDetails : " + ex.Message);
            }
            finally
            {
            }
            return objTransctionResponse;
        }


        public PanDetailsResponse GetPanDetails(PanDetailsRetrieverequest ObjPanDetailsRetrieverequest)
        {
            PanDetailsResponse ObjPanDetailsResponse = null;
            try
            {
                ObjPanDetailsResponse = new SeMobileAppBO().GetPanDetails(ObjPanDetailsRetrieverequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " GetPanDetails() " + ex.Message);
            }
            return ObjPanDetailsResponse;
        }


        public PanDetailsResponse SaveCustomerPanDetails(PanDetailsRetrieverequest ObjPanDetailsRetrieverequest)
        {
            PanDetailsResponse ObjPanDetailsResponse = null;
            string FullPath = string.Empty;
            string FileName = string.Empty;
            try
            {
                FullPath = HTTP_UPLOAD_FOLDER_PATH + IDENTITY_PROOF_PIC_FOLDER_PATH;

                //Display Image
                if (!string.IsNullOrEmpty(ObjPanDetailsRetrieverequest.PanImage))
                {
                    FileName = ObjPanDetailsRetrieverequest.LoyaltyId + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".png";
                    bool Result = UICommon.SaveBase64StringIntoFile(FullPath.Replace("~", ""), ObjPanDetailsRetrieverequest.PanImage, FileName);
                    ObjPanDetailsRetrieverequest.PanImage = IDENTITY_PROOF_PIC_FOLDER_PATH.Replace("\\", "/") + FileName;
                }
                ObjPanDetailsResponse = new SeMobileAppBO().SaveCustomerPanDetails(ObjPanDetailsRetrieverequest);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " SaveCustomerPanDetails() " + ex.Message);
            }
            return ObjPanDetailsResponse;
        }
        public PanDetailsResponse ValidatePanDetails(PanDetailsRetrieverequest ObjPanDetailsRetrieverequest)
        {
            PanDetailsResponse ObjPanDetailsResponse = null;
            try
            {
                ObjPanDetailsResponse = new PanDetailsResponse();
                ObjPanDetailsResponse.ObjPanDetailsRetrieverequest = ValidatePanDetailsWithNSDL(ObjPanDetailsRetrieverequest.PanNumber);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " ValidatePanDetails() " + ex.Message);
            }
            return ObjPanDetailsResponse;
        }

        private PanDetailsRetrieverequest ValidatePanDetailsWithNSDL(string PanNumber)
        {
            PanDetailsRetrieverequest ObjPanDetailsRetrieverequest = null;
            try
            {
                string PanToken = string.Empty;
                string TokenUrl = string.Empty;
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                TokenUrl = PAN_TOKEN_GENERATE_URL;
                HttpRequestArguments httpRequestArguments = new HttpRequestArguments(TokenUrl, "POST", "application/json", false, "");
                httpRequestArguments.Headers = new WebHeaderCollection();

                httpRequestArguments.Headers.Add("x-api-key", PAN_API_KEY);
                httpRequestArguments.Headers.Add("x-api-secret", PAN_API_SECRET_KEY);
                httpRequestArguments.Headers.Add("x-api-version", PAN_API_VERSION);
                PanToken = HttpRequest.MakePostRequest(httpRequestArguments);
                if (!string.IsNullOrEmpty(PanToken))

                {
                    JObject response = JObject.Parse(PanToken);
                    PanToken = response["access_token"].ToString();

                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                    string GstURL = PAN_VALIDATE_URL + PanNumber + "/verify?consent=Y&reason=For KYC of User";
                    HttpRequestArguments httpRequestArgumentGst = new HttpRequestArguments(GstURL, "", "application/json", false, "");
                    httpRequestArgumentGst.Headers = new WebHeaderCollection();
                    httpRequestArgumentGst.Headers.Add("Authorization", PanToken);
                    httpRequestArgumentGst.Headers.Add("x-api-key", PAN_API_KEY);
                    httpRequestArgumentGst.Headers.Add("x-api-version", PAN_API_VERSION);
                    string ResponseMessage = HttpRequest.MakeGetRequest(httpRequestArgumentGst);
                    ObjPanDetailsRetrieverequest = new PanDetailsRetrieverequest();
                    if (!string.IsNullOrEmpty(ResponseMessage))
                    {
                        PanApiResponse objPanApiResponse = (PanApiResponse)JsonConvert.DeserializeObject(ResponseMessage, typeof(PanApiResponse));

                        if (objPanApiResponse != null)
                        {
                            if (objPanApiResponse.data != null)
                            {
                                if (objPanApiResponse.data.status.Equals("VALID"))
                                {
                                    ObjPanDetailsRetrieverequest.PanNumber = objPanApiResponse.data.pan;
                                    ObjPanDetailsRetrieverequest.FirstName = objPanApiResponse.data.full_name;
                                    ObjPanDetailsRetrieverequest.IsPanValid = 1;
                                }
                                else if (objPanApiResponse.data.status.Equals("INVALID"))
                                {
                                    ObjPanDetailsRetrieverequest.IsPanValid = -3;
                                }
                            }
                        }
                        else
                        {
                            ObjPanDetailsRetrieverequest.IsPanValid = -3; // if data is null 
                        }
                    }
                    else
                    {
                        ObjPanDetailsRetrieverequest.IsPanValid = -3; // if data is null 
                    }

                }
                else
                {
                    ObjPanDetailsRetrieverequest = new PanDetailsRetrieverequest();
                    ObjPanDetailsRetrieverequest.IsPanValid = -1; // technical error Ex: if Subsccription ended
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(SERVICE_CLASS_NAME + " ValidatePanDetailsWithNSDL() " + ex.Message);
            }
            return ObjPanDetailsRetrieverequest;
        }
    }
}
