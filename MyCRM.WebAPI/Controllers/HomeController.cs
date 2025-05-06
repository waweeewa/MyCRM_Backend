using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyCRM.Common;
using MyCRM.DAL.DataModel;
using MyCRM.Model;
using MyCRM.Repository.Common;
using MyCRM.Service;
using MyCRM.Service.Common;
using MyCRM.WebAPI.RESTModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;

namespace MyCRM.WebAPI.Controllers
{
    [Route("pis")]
    public class HomeController : Controller
    {
        protected IService _service { get; private set; }
        protected IRepository _repository { get; private set; }
        private int _requestUserId;

        public HomeController(IService service, IRepository repository)
        {
            _service = service;
            _repository = repository;
            _requestUserId = -1;
        }
        [HttpGet]
        [Route("test")]
        public string Test()
        {
            return _service.Test();
        }
        [HttpGet]
        [Route("Users_db")]
        public IEnumerable<PisUsersDResetar> GetAllUsersDb()
        {
            IEnumerable<PisUsersDResetar> userDb = _service.GetAllUsersDb();

            return userDb;
        }
        [HttpGet]
        [Route("Users")]
        public async Task<IActionResult> GetAllUsers()
        {

            HttpRequestResponse<IEnumerable<UsersDomain>> response = new HttpRequestResponse<IEnumerable<UsersDomain>>();

            Tuple<IEnumerable<UsersDomain>, List<ErrorMessage>> result = await _service.GetAllUsers();

            if (result != null)
            {
                response.Result = result.Item1;
                response.ErrorMessages = result.Item2;
                return Ok(response);
            }
            else
            {
                response.Result = result.Item1;
                response.ErrorMessages = result.Item2;
                return Ok(response);
            }


        }
        [HttpGet]
        [Route("Users/user_id/{userId}")]
        public async Task<IActionResult> GetUserDomainByUserId(int userId)
        {
            HttpRequestResponse<UsersDomain> response = new HttpRequestResponse<UsersDomain>();

            Tuple<UsersDomain, List<ErrorMessage>> result = await _service.GetUserDomainByUserId(userId);

            if (result != null)
            {
                response.Result = result.Item1; // Assign the single UsersDomain object directly
                response.ErrorMessages = result.Item2;
                return Ok(response);
            }
            else
            {
                // Handle the case where result is null, if needed
                return NotFound(); // Or another appropriate response
            }
        }
        [HttpPost]
        [Route("Users/add")]
        public async Task<IActionResult> AddUserAsync([FromBody] PisUsersDResetar userRest)
        {
            //bool lastrequestId = await GetLastUserRequestId();

            //if (!lastrequestId)
            //{
            //    return BadRequest("Nije unesen RequestUserId korisnika koji poziva.");

            //}
            //else
            //{

                try
                {
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(ModelState);
                    }
                    else
                    {
                        PisUsersDResetar userDomain = new PisUsersDResetar();
                        userDomain.firstName = userRest.firstName;
                        userDomain.lastName = userRest.lastName;
                        userDomain.address = userRest.address;
                        userDomain.zipcode = userRest.zipcode;
                        userDomain.admincheck = userRest.admincheck;
                        userDomain.email = userRest.email;
                        userDomain.birthdate = userRest.birthdate;
                        userDomain.city = userRest.city;
                        userDomain.country = userRest.country;
                        userDomain.password = userRest.password;
                        userDomain.tariffId = userRest.tariffId;

                        bool add_user = await _service.AddUserAsync(userDomain);

                        if (add_user)
                        {

                            return Ok("User dodan!");
                        }
                        else
                        {
                            return Ok("User nije dodan!");
                        }
                    }

                }
                catch (Exception e)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, e.ToString());
                }
            }
        [HttpGet]
        [Route("Users/{userId}/years")]
        public async Task<IActionResult> GetBillingYears(int userId)
        {
            HttpRequestResponse<IEnumerable<int>> response = new HttpRequestResponse<IEnumerable<int>>();

            IEnumerable<int> result = _service.GetBillingYears(userId);

            if (result != null)
            {
                response.Result = result;
                return Ok(response);
            }
            else
            {
                return NotFound();
            }
        }
        [HttpGet]
        [Route("Users/{userId}/{year}/dashboard")]
        public async Task<IActionResult> GetDashboardData(int userId, int year)
        {
            HttpRequestResponse<IEnumerable<int>> response = new HttpRequestResponse<IEnumerable<int>>();

            List<int> result = await _service.GetDashboardData(userId, year);

            if (result != null)
            {
                response.Result = result;
                return Ok(response);
            }
            else
            {
                return NotFound();
            }
        }
        [HttpGet]
        [Route("Users/{userId}/{month_from}/{year_from}/{month_to}/{year_to}/{mode}/report")]
        public async Task<IActionResult> GetReport(int userId, int month_from, int year_from, int month_to, int year_to, string mode)
        {
            HttpRequestResponse<IEnumerable<int>> response = new HttpRequestResponse<IEnumerable<int>>();

            List<int> result = await _service.GetReport(userId, month_from, year_from, month_to, year_to, mode);

            if (result != null)
            {
                response.Result = result;
                return Ok(response);
            }
            else
            {
                return NotFound();
            }
        }
        [HttpGet]
        [Route("Users/{userId}/{month_from}/{year_from}/{month_to}/{year_to}/{mode}/reportTariffArchive")]
        public async Task<IActionResult> GetReportTariffArchive(int userId, int month_from, int year_from, int month_to, int year_to, string mode)
        {
            var result = await _repository.GetReportWithTariffArchive(userId, month_from, year_from, month_to, year_to, mode);
            if(result != null)
            {
                return Ok(result);
            }
            else
            {
                return NotFound();
            }
        }
        [HttpGet]
        [Route("Users/{userId}")]
        public async Task<IActionResult> GetUser(int userId)
        {
            HttpRequestResponse<PisUsersDResetar> response = new HttpRequestResponse<PisUsersDResetar>();

            PisUsersDResetar result = _service.GetUser(userId);

            if (result != null)
            {
                response.Result = result;
                return Ok(response);
            }
            else
            {
                return NotFound();
            }
        }
        [HttpPut]
        [Route("Users/put/{userId}")]
        public async Task<IActionResult> PutUser([FromBody] PisUsersDResetar user)
        {
            //bool lastrequestId = await GetLastUserRequestId();

            //if (!lastrequestId)
            //{
            //    return BadRequest("Nije unesen RequestUserId korisnika koji poziva.");

            //}
            //else
            //{
                try
                {
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(ModelState);
                    }
                    else
                    {
                        PisUsersDResetar userDomain = new PisUsersDResetar();
                        userDomain.uId = user.uId;
                        userDomain.firstName = user.firstName;
                        userDomain.lastName = user.lastName;
                        userDomain.address = user.address;
                        userDomain.zipcode = user.zipcode;
                        userDomain.admincheck = user.admincheck;
                        userDomain.email = user.email;
                        userDomain.birthdate = user.birthdate;
                        userDomain.city = user.city;
                        userDomain.country = user.country;
                        userDomain.password = user.password;
                        userDomain.tariffId = user.tariffId;

                        bool put_user = await _repository.PutUser(userDomain);

                        if (put_user)
                        {

                            return Ok("User promijenjen!");
                        }
                        else
                        {
                            return Ok("User nije promijenjen!");
                        }
                    }

                }
                catch (Exception e)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, e.ToString());
                }
            }
        [HttpDelete]
        [Route("Users/delete/{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            //bool lastrequestId = await GetLastUserRequestId();

            //if (!lastrequestId)
            //{
            //    return BadRequest("Nije unesen RequestUserId korisnika koji poziva.");

            //}
            //else
            //{
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                else
                {
                    bool delete_user = await _repository.DeleteUser(userId);

                    if (delete_user)
                    {

                        return Ok("User obrisan!");
                    }
                    else
                    {
                        return Ok("User nije obrisan!");
                    }
                }

            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.ToString());
            }
        }

        [HttpPost]
        [Route("Device/add")]
        public async Task<IActionResult> AddDevice([FromBody] VerifiedDevice credentials)
        {
            _repository.AddDevice(credentials);
            return Ok();
        }
        [HttpPut]
        [Route("Device/put")]
        public async Task<IActionResult> PutDevice([FromBody] VerifiedDevice credentials)
        {
            _repository.PutDevice(credentials);
            return Ok();
        }
        [HttpDelete]
        [Route("Device/delete")]
        public async Task<IActionResult> DeleteDevice([FromBody] VerifiedDevice credentials)
        {
            _repository.DeleteDevice(credentials);
            return Ok();
        }
        [HttpGet]
        [Route("Device")]
        public async Task<IActionResult> GetDevices()
        {

            HttpRequestResponse<IEnumerable<VerifiedDevice>> response = new HttpRequestResponse<IEnumerable<VerifiedDevice>>();
            IEnumerable<VerifiedDevice> result = _repository.GetDevices();
            if (result != null)
            {
                response.Result = result;
                return Ok(response);
            }
            else
            {
                return NotFound();
            }
        }
        [HttpPost]
        [Route("Tariff/add")]
        public async Task<IActionResult> AddTariff([FromBody] Tariff credentials)
        {
            _repository.AddTariff(credentials);
            return Ok();
        }
        [HttpDelete]
        [Route("Tariff/delete")]
        public async Task<IActionResult> DeleteTariff([FromBody] Tariff credentials)
        {
            _repository.DeleteTariff(credentials);
            return Ok();
        }
        [HttpGet]
        [Route("Tariff")]
        public async Task<IActionResult> GetTariff()
        {

            HttpRequestResponse<IEnumerable<Tariff>> response = new HttpRequestResponse<IEnumerable<Tariff>>();
            IEnumerable<Tariff> result = _repository.GetTariff();
            if (result != null)
            {
                response.Result = result;
                return Ok(response);
            }
            else
            {
                return NotFound();
            }
        }
        [HttpPut]
        [Route("Tariff/put/{month}/{year}/{price}/{tariffId}")]
        public async Task<IActionResult> PutTariff(int month, int year, decimal price, int tariffId)
        {
            var result = await _service.UpdateTariffAndArchive(month, year, price, tariffId);
            if (result)
            {
                return Ok("Tariff and Archive updated.");
            }
            return NotFound("Tariff not found.");
        }
        [HttpGet]
        [Route("Users/{userId}/bills")]
        public async Task<IActionResult> GetUserBills(int userId)
        {
            HttpRequestResponse<IEnumerable<Billing>> response = new HttpRequestResponse<IEnumerable<Billing>>();

            IEnumerable<Billing> result = _service.GetUserBills(userId);

            if (result != null)
            {
                response.Result = result;
                return Ok(response);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginCredentials credentials)
        {
            Tuple<UsersDomain, List<ErrorMessage>> isAuthenticated = await _service.AuthenticateUser(credentials.email, credentials.password);

            return Ok(isAuthenticated);
        }

        [HttpGet]
        [Route("Users/{userId}/bills/{month}/{year}/{loguserId}/")]
        public async Task<IActionResult> BillExport(int userId, int month, int year, int loguserId)
        {
            BillExport bEx = await _repository.BillExport(userId, month, year, loguserId);
            string pdfFilePath = GeneratePDF(bEx);

            if (System.IO.File.Exists(pdfFilePath))
            {
                var stream = new FileStream(pdfFilePath, FileMode.Open);
                return new FileStreamResult(stream, "application/pdf")
                {
                    FileDownloadName = $"BillExport_{month}_{year}.pdf"
                };
            }
            else
            {
                return NotFound();
            }
        }
        [HttpGet]
        [Route("Invoice/{loguserId}")]
        public async Task<IActionResult> GetInvoice(int loguserId)
        {
            HttpRequestResponse<IEnumerable<Invoices>> response = new HttpRequestResponse<IEnumerable<Invoices>>();
            IEnumerable<Invoices> result = _repository.GetInvoices(loguserId);
            return Ok(result);
        }
        [HttpPost]
        [Route("Invoice/post/{userId}/{month}/{year}/{usedPower}/{tariffId}/{deviceId}")]
        public async Task<IActionResult> PostInvoice(int userId, int month, int year, int usedPower, int tariffId, int deviceId)
        {
            Billing credentials = new Billing() { UserId = userId, Month = month, Year = year, UsedPower = usedPower, TarriffId = tariffId, DeviceId = deviceId };
            Task<bool> result = _repository.PostInvoice(credentials, tariffId);
            return Ok(result);
        }
        [HttpGet]
        [Route("Invoice/{userId}/{month}/{year}")]
        public async Task<IActionResult> GetInvoiceById(int userId,int month,int year)
        {
            HttpRequestResponse<InvoiceGET> response = new HttpRequestResponse<InvoiceGET>();
            Task<InvoiceGET> result = _repository.GetInvoiceByMonth(userId, month, year);
            return Ok(result);
        }
        [HttpPut]
        [Route("Invoice/put/{billId}/{userId}/{email}/{month}/{year}/{usedPower}/{paidAmount}/{deviceId}/{tariffId}")]
        public async Task<IActionResult> PutInvoice(int billId, int userId, string email, int month, int year, int usedPower, decimal paidAmount, int deviceId, int tariffId)
        {
            Billing credentials = new Billing() { BId = billId, UserId = userId, Month = month, Year = year, UsedPower = usedPower, Paid = paidAmount, TarriffId = tariffId, DeviceId = deviceId };
            _repository.PutInvoice(credentials);
            return Ok();
        }
        [HttpDelete]
        [Route("Invoice/delete/{userId}/{month}/{year}")]
        public async Task<IActionResult> DeleteInvoice(int userId,int month, int year)
        {
            _repository.DeleteInvoice(userId, month, year);
            return Ok();
        }

        [HttpPut]
        [Route("Device/{deviceId}/{day_from}/{month_from}/{year_from}/{day_to}/{month_to}/{year_to}")]
        public async Task<IActionResult> PutUserDevice(int deviceId, int day_from, int month_from, int year_from, int day_to, int month_to, int year_to)
        {
            _repository.PutUserDevice(deviceId, day_from, month_from, year_from, day_to, month_to, year_to);
            return Ok();
        }
        [HttpPost]
        [Route("UserDevice/post/")]
        public async Task<IActionResult> AddUserDevice([FromBody] UserDeviceModel userDevice)
        {
            _service.AddUserDevice(userDevice);
            return Ok();
        }
        [HttpDelete]
        [Route("UserDevice/delete/{udId}")]
        public async Task<IActionResult> DeleteUserDevice(int udId)
        {
            _repository.DeleteUserDevice(udId);
            return Ok();
        }
        [HttpGet]
        [Route("UserDevices/")]
        public async Task<IActionResult> GetUserDevices()
        {
            HttpRequestResponse<IEnumerable<UserDevice>> response = new HttpRequestResponse<IEnumerable<UserDevice>>();
            IEnumerable<UserDeviceModel> result = _repository.GetUserDevices();
            return Ok(result);
        }
        [HttpGet]
        [Route("Users/{userId}/highest-usage-month")]
        public async Task<IActionResult> GetMonthWithHighestUsage(int userId)
        {
            int highestUsageMonth = await _repository.GetMonthWithHighestUsage(userId);
            
            if (highestUsageMonth == 0)
                return NotFound("No consumption data available for this user");
            
            // Convert month number to month name
            string monthName = new DateTime(2020, highestUsageMonth, 1).ToString("MMMM");
            
            return Ok(new { 
                monthNumber = highestUsageMonth, 
                monthName = monthName
            });
        }
        #region AdditionalCustomFunctions
        public async Task<bool> GetLastUserRequestId()
        {
            IHeaderDictionary headers = this.Request.Headers;
            if (headers.ContainsKey("RequestUserId"))
            {
                if (int.TryParse(headers["RequestUserId"].ToString(), out _requestUserId))
                {
                    return await _service.IsValidUser(_requestUserId);
                    //return true;
                }
                else return false;
            }
            return false;

        }
        [HttpGet]
        [Route("Available/{userId}/{month}/{year}")]
        public async Task<IActionResult> Available(int userId, int month, int year)
        {
            // Create a date range for the specified month and year
            DateTime startDate = new DateTime(year, month, 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);

            // Query user devices within the specified date range
            IEnumerable<UserDeviceModel> result = _repository.GetUserDevices()
                .Where(d => d.userId == userId &&
                            DateTime.Parse(d.from_date) <= endDate &&
                            DateTime.Parse(d.to_date) >= startDate)
                .ToList();

            return Ok(result);
        }
        private string GeneratePDF(BillExport billExport)
        {
            string webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            string fileName = $"BillExport_{billExport.month}_{billExport.year}.pdf";
            string filePath = Path.Combine(webRootPath, fileName);

            // Ensure the directory exists
            if (!Directory.Exists(webRootPath))
            {
                Directory.CreateDirectory(webRootPath);
            }

            using (Document document = new Document(PageSize.A4, 50, 50, 25, 25))
            {
                PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
                document.Open();

                // Add a logo
                string solutionPath = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
                string logoPath = Path.Combine(solutionPath, "Images", "logo.png");

                if (System.IO.File.Exists(logoPath))
                {
                    iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(logoPath);
                    logo.ScaleAbsolute(100f, 50f);
                    logo.SetAbsolutePosition(20f, document.PageSize.Height - 60f);
                    document.Add(logo);
                }

                // Add a title
                Font titleFont = FontFactory.GetFont("Arial", 10);
                Paragraph title = new Paragraph("OIB: 47193857382\nElectricity distribution company - Sample City\nSample Street 12\nFAX: 035 / 481 - 813", titleFont)
                {
                    Alignment = Element.ALIGN_LEFT,
                    SpacingBefore = 30f // Adjust the spacing here as needed
                };
                document.Add(title);


                AddRectangle(document);


                document.Add(new Paragraph("\n"));

                // Add bill information in a table
                PdfPTable table = new PdfPTable(2)
                {
                    WidthPercentage = 100,
                    SpacingBefore = 20f,
                    SpacingAfter = 30f
                };
                table.SetWidths(new float[] { 1, 2 });

                AddTableCell(table, "Customer Name:", billExport.firstName + " " + billExport.lastName);
                AddTableCell(table, "Billing Month:", $"{billExport.month}/{billExport.year}");
                AddTableCell(table, "Used Power:", $"{billExport.usedPower} kWh");
                AddTableCell(table, "Old Balance:", $"{billExport.totalAmount:C}");
                AddTableCell(table, "New Balance:", $"{billExport.paidAmount:C}");
                AddTableCell(table, "Unit Price:", $"{billExport.unitPrice:C}");

                document.Add(table);

                // Add device information
                AddDeviceInformation(document, billExport);

                // Add additional details or disclaimers
                Font detailFont = FontFactory.GetFont("Arial", 10, Font.NORMAL);
                Paragraph details = new Paragraph("Please ensure to pay the total amount by the due date to avoid any service interruptions.", detailFont);
                details.Alignment = Element.ALIGN_LEFT;
                document.Add(details);

                // Add contact information or footer
                Paragraph footer = new Paragraph("For any queries, please contact us at support@electricitycompany.com or call 123-456-7890.", detailFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingBefore = 20f
                };
                document.Add(footer);

                document.Close();
            }

            return filePath;
        }
        private void AddRectangle(Document document)
        {
            // Create a rectangle with specified dimensions
            Rectangle rectangle = new Rectangle(100f, 100f, 200f, 200f);

            // Set the color of the rectangle
            rectangle.BackgroundColor = BaseColor.LIGHT_GRAY;

            // Add the rectangle to the document
            document.Add(rectangle);
        }

        private void AddTableCell(PdfPTable table, string text, string value)
        {
            Font font = FontFactory.GetFont("Arial", 12, Font.NORMAL);
            PdfPCell cell = new PdfPCell(new Phrase(text, font))
            {
                Border = PdfPCell.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(value, font))
            {
                Border = PdfPCell.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            table.AddCell(cell);
        }
        private void AddDeviceInformation(Document document, BillExport billExport)
        {
            // Add device information in a table
            PdfPTable table = new PdfPTable(2)
            {
                WidthPercentage = 100,
                SpacingBefore = 20f,
                SpacingAfter = 30f
            };
            table.SetWidths(new float[] { 1, 2 });
            List<BillExport.Device> devices = billExport.devices;

            // Add table header
            Font headerFont = FontFactory.GetFont("Arial", 12, Font.BOLD);
            PdfPCell deviceNameHeaderCell = new PdfPCell(new Phrase("Device Name", headerFont))
            {
                Border = PdfPCell.BOTTOM_BORDER,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            table.AddCell(deviceNameHeaderCell);

            PdfPCell usedPowerHeaderCell = new PdfPCell(new Phrase("Total Power Used", headerFont))
            {
                Border = PdfPCell.BOTTOM_BORDER,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            table.AddCell(usedPowerHeaderCell);

            foreach (var device in devices)
            {
                Font deviceNameFont = FontFactory.GetFont("Arial", 15, Font.BOLD);
                PdfPCell deviceNameCell = new PdfPCell(new Phrase(device.Name, deviceNameFont))
                {
                    Border = PdfPCell.TOP_BORDER | PdfPCell.BOTTOM_BORDER,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(deviceNameCell);

                PdfPCell usedPowerCell = new PdfPCell(new Phrase(device.PowerUsage.ToString() + "kW", FontFactory.GetFont("Arial", 15)))
                {
                    Border = PdfPCell.TOP_BORDER | PdfPCell.BOTTOM_BORDER,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(usedPowerCell);
            }

            document.Add(table);
        }

        [HttpGet]
        [Route("electricity-summary/{userId}")]
        public async Task<IActionResult> GetElectricitySummaryData(int userId)
        {
            var summary = await _repository.GetElectricitySummaryExport(userId);
            var response = new
            {
                LastMonthConsumption = summary.Item1,
                LastMonthPrice = summary.Item2,
                CurrentYearConsumption = summary.Item3,
                CurrentYearPrice = summary.Item4,
                LastYearConsumption = summary.Item5,
                LastYearPrice = summary.Item6,
                MonthlyConsumption = summary.Item7,
                ConsumptionDiff = summary.Item8,
                WinterAverage = summary.Item9,
                SummerAverage = summary.Item10,
                MedianLastYearConsumption = summary.Item11,
                TrendPrediction = summary.Item12,
                ModMonthName = summary.Item13
            };
            return Ok(response);
        }

        [HttpGet]
        [Route("electricity-export/{userId}")]
        public async Task<IActionResult> ExportElectricitySummary(int userId)
        {
            var summary = await _repository.GetElectricitySummaryExport(userId);
            return Ok(summary);
        }
        }
        #endregion AdditionalCustomFunctions
    }