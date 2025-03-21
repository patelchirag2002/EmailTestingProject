using EmailTestingAPI.Models.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using static SwagatDecor.Common.EmailNotification.EmailNotification;

namespace EmailTestingAPI.Controllers
{
    [Route("api/email")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly SMTPSettings _smtpSettings;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        #region Constructor
        public EmailController(IOptions<SMTPSettings> smtpSettings,
            IWebHostEnvironment hostingEnvironment,
            IHttpContextAccessor httpContextAccessor)
        {
            _smtpSettings = smtpSettings.Value;
            _hostingEnvironment = hostingEnvironment;
            _httpContextAccessor = httpContextAccessor;
        }
        #endregion

        [HttpGet("send")]
        public async Task<bool> SendEmail()
        {
            EmailSetting setting = new EmailSetting
            {
                EmailEnableSsl = Convert.ToBoolean(_smtpSettings.EmailEnableSsl),
                EmailHostName = _smtpSettings.EmailHostName,
                EmailPassword = _smtpSettings.EmailPassword,
                EmailAppPassword = _smtpSettings.EmailAppPassword,
                EmailPort = Convert.ToInt32(_smtpSettings.EmailPort),
                FromEmail = _smtpSettings.FromEmail,
                FromName = _smtpSettings.FromName,
                EmailUsername = _smtpSettings.EmailUsername,
            };

            string emailBody = string.Empty;
            string BasePath = Path.Combine(_hostingEnvironment.WebRootPath, "EmailTemplates");
            string path = _httpContextAccessor.HttpContext.Request.Scheme + "://" + _httpContextAccessor.HttpContext.Request.Host.Value;

            if(!Directory.Exists(BasePath))
            {
                Directory.CreateDirectory(BasePath);
            }
            bool isSuccess = false;

            using (StreamReader reader = new StreamReader(Path.Combine(BasePath, "index.html")))
            {
                emailBody = reader.ReadToEnd();
            }

            isSuccess = await Task.Run(() => SendMailMessage("kinnari.p@shaligraminfotech.com", null, null, "Testing Email", emailBody, setting, null));
            if (isSuccess)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
