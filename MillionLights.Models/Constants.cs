using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Millionlights.Models
{
   public class Constants
    {
        //CCAvenue Account Details

       //CCAvenue Payment Gateway Details - Test Environment - LocalHost:8094
       public const string TestWorkingKey = "AB50AEC0D366360D0323865FE5656757";
       public const string TestStrAccessCode = "AVEB07CK70CF55BEFC";
       public const string TestMerchantId = "80592";

       //CCAvenue Payment Gateway Details - Live Environment
       public const string LiveWorkingKey = "AB50AEC0D366360D0323865FE5656757";
       public const string LiveTestStrAccessCode = "AVEB07CK70CF55BEFC";
       public const string LiveTestMerchantId = "80592";

       //
       public const string UserRegSubNotification = "You have been successfully registered.";
       public const string StudentRegSubNotification = "You have been successfully registered.";
       public const string ExtUserRegSubNotification = "You have been successfully registered. Please edit your profile and add phone number.";
       public const string UserRegMsgNotification = "Your registration with Edunetworks is successful. Your login id {0} and Password is {1}";
       public const string StudentRegMsgNotification = "Your registration with Edunetworks is successful. Your login id {0}";
       public const string ExtUserRegMsgNotification = "Your registration with Edunetworks is successful. Your login id {0}";
       public const string ForgotPwdSubNotification = "Your password reset request has been sent to you successfully.";
       public const string ForgotPwdMsgNotification = "Please note, a link to reset your password has been sent to you.In case of any issues, contact Edunetworks Support at support@edunetworks.com ";
       public const string ResetPwdSubNotification = "Your Password reset successfully";
       public const string ResetPwdMsgNotification = "Please note, your password has been reset as requested by you.In case of any issues, contact Edunetworks Support at support@edunetworks.com";
       public const string CouponSubNotification = "Coupon have been created successfully";
       public const string CouponMsgNotification = "Message - Coupon Created Successfully";
       public const string CourseCouponEnrollSubNotification = "Your Order {0} is successful";
       public const string CourseCouponEnrollMsgNotification = "Your order {0} has been successfully placed for the following course(s): {1}";
       public const string UserRegistrationSuccessMsg = "Your Edunetworks registration is successful. Loginid is {0} and Password is {1}";
       public const string StudentRegistrationSuccessMsg = "Your Edunetworks registration is successful. Loginid is {0}";
       public const string UserPasswordChange = "Your Edunetworks password has been changed successfully. Your LoginId is {0}";
       public const string ForgotPasswordTextMsg= "The link to reset the password is sent to your email Id {0}";
       public const string RecordSave = "Record saved successfully";
       public const string OrderFailSMSMsg = "Your order {0} on Edunetworks has failed.";
       public const string OrderSuccessSMSMsg = "Your order {0} on Edunetworks is successful";
       public const string ContactUsSMSMsg = "Thank you for contacting Edunetworks. We will get back to you soon.";
       public const string AssignedCouponSMSMsg = "Dear {0}, Congratulations!! you can enroll {1} courses from {2} use coupon Code : {3} to get discount : {4} percent On above courses.";
       //public const string ResetPasswordLink = "http://localhost:8094/Account/ResetMyPassword?verificationId={0}";
        public const string ResetPasswordLink = "http://ml-dev5.azurewebsites.ne/Account/ResetMyPassword?verificationId={0}";
        //public const string ResetPasswordLink = "https://www.millionlights.org/Account/ResetMyPassword?verificationId={0}";
        public const string RefCodeSharedSuccessMsgFromSenderToReceiver = "Your friend {0} has invited you to join the online course on Millionlights. Use {1} code to register and earn Rs. {2} off on any course and get certified.";
       public const string RefCodeRedeemBenefitSuccessMsgToReceiver = "Referral code {0} successfully applied. Rs.{1} has been added to your wallet. Share your referral code and keep earning more credits to your wallet.";
       public const string RefCodeRedeemBenefitSuccessMsgToReferrer = "Your friend {0} has used the code {1} you shared. Rs.{2} has been added to your wallet. Share more and keep earning more credits to your wallet.";


       public static Dictionary<string, string> Messages()
       {
           Dictionary<string, string> messages = new Dictionary<string, string>();
           //messages.Add("InvalidUser", "The username / password you have entered are invalid. Please enter valid username/password.");
           messages.Add("InvalidUser", "The username or password you have entered is invalid. Please try again.");
           messages.Add("NeedCouponActivateMsg", "Login to activate the coupon");
           messages.Add("NeedRegForCouponActivateMsg", "Please Register or Login to Activate Coupon code / Referral code.");
           messages.Add("CouponInvalid", "The Coupon you entered has been already redeemed. You can verify the coupon status in My Account or Contact us at support@edunetworks.com .");
           messages.Add("SendPassword", "Password sent to your registered email.");
           messages.Add("ValidRegEmailId", "Please enter valid/registered email ID.");
           messages.Add("EnterEmailId", " Enter your email address");
           messages.Add("IncorrectEmailInSystem", "We could not find the email address in our system. Please enter correct email address or register.");
           messages.Add("AlredyEnroll", "You have already enrolled in this Course. Please add another course Instead.");
           messages.Add("ActivateCouponAlredyEnroll", "Please check your account [My Courses] and un-select the already enrolled course(s) to proceed for course enrollment.");
           messages.Add("OrderCourseAlredyEnroll", "Please delete the already enrolled course(s) to proceed for course enrollment.");
           messages.Add("Disable", "Are you sure you want to disable the record");
           messages.Add("Enable", "Do you want to enable the record?");
           messages.Add("SelectAnyField", "Please select any one field from above");
           messages.Add("DeleteRecord", "Do you want to delete the record?");
           messages.Add("DeleteSelectRecord", "Are you sure you want to delete the selected record");
           messages.Add("QuerySubmit", "Thank you for showing the interest, We have submitted your query and will getback to you soon!");
           messages.Add("NotSendEmailForIssue", "Sorry, there was some issue with the email service. We are not able to send emails. Please try after some time or raise a support request.");
           messages.Add("EmailSucess", "The Email Campaign was successfully submitted for sending emails.");
           messages.Add("NotSendSMSForIssue", "Sorry, we cant send your sms due to some internal issues. Please try later!.");
           messages.Add("SMSSucess", "Congratulations! Your sms has been sent successfully!");
           messages.Add("CancelOrder", "Are you sure you want to cancel the order?");
           messages.Add("RefundOrder", "Are you sure you want to refund the order?");
           messages.Add("ifExternalUser", "We can’t reset Social Login passwords");
           messages.Add("InvalidCoupon","Enter valid Coupon code / Referral code?");
           messages.Add("ExternalUserAlreadyExist", "Sorry, the account is already registered with ");
           return messages;
       }
      
    }
}
