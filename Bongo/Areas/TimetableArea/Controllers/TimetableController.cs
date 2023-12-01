using Bongo.Areas.TimetableArea.Infrastructure;
using Bongo.Areas.TimetableArea.Models;
using Bongo.Areas.TimetableArea.Models.ViewModels;
using Bongo.Data;
using Bongo.Models;
using Bongo.Services;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using System.Text.RegularExpressions;
using PdfDocument = Syncfusion.Pdf.PdfDocument;
using PdfPage = Syncfusion.Pdf.PdfPage;

namespace Bongo.Areas.TimetableArea.Controllers
{
    [MyAuthorize]
    [Area("TimetableArea")]
    public class TimetableController : Controller
    {
        private IEndpointWrapper wrapper;
        private readonly IMailService _mailSender;
        private readonly UserManager<BongoUser> _userManager;

        public TimetableController(IEndpointWrapper _wrapper, IMailService mailSender, UserManager<BongoUser> userManager)
        {
            wrapper = _wrapper;
            _mailSender = mailSender;
            _userManager = userManager;
        }

        #region Hepler Methods
        public string GetTime(int i)
        {
            string starthourtime = (i + 7) > 9 ? (i + 7).ToString() : "0" + (i + 7).ToString();
            string endhourtime = (i + 8) > 9 ? (i + 8).ToString() : "0" + (i + 8).ToString();
            string time = $"{starthourtime}:00";
            time += Request.Cookies["isTimeSingle"].ToString() == "false" ? $" -{endhourtime}:00" : "";
            return time;
        }
        public void SetCookie(string key, string value)
        {
            CookieOptions cookieOptions = new CookieOptions { Expires = DateTime.Now.AddDays(90) };
            Response.Cookies.Append(key, value == null ? "" : value, cookieOptions);
        }
        #endregion

        [HttpGet]
        public async Task<IActionResult> Display(int latestPeriod = 0)
        {

            if (Request.Cookies["isVenueHidden"] == null)
            {
                SetCookie("isVenueHidden", "false");
            }

            if (Request.Cookies["isTimeSingle"] == null)
            {
                SetCookie("isTimeSingle", "true");
            }

            bool isForFirstSemester = Request.Cookies["isForFirstSemester"] == "true";
            ViewBag.isForFirstSemester = isForFirstSemester;
            TempData["isForFirstSemester"] = isForFirstSemester;

            var response = await wrapper.Session.GetTimetableSessions(isForFirstSemester);
            switch ((int)response.StatusCode)
            {
                case 200:
                    goto valid;
                case 400:
                    return RedirectToAction("Manage", "Session");
                default:
                    return View("Upload");
            }


        valid:
            IndexViewModel indexViewModel = new IndexViewModel
            {
                Sessions = SessionControlHelpers.Get2DArray(await response.Content.ReadFromJsonAsync<List<Session>>()),
                latestPeriod = latestPeriod
            };
            SetCookie("latestPeriod", indexViewModel.latestPeriod.ToString());
            return View(indexViewModel);
        }
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file, string[] startBlank, bool isFirstSemester = true)
        {
            if (file != null)
            {
                if (file.Length < 500000) //revise
                {
                    if (file.ContentType == "application/pdf")
                    {
                        string text = "";
                        try
                        {
                            using (Stream stream = file.OpenReadStream())
                            {
                                //Read the text from a pdf file
                                using (iText.Kernel.Pdf.PdfDocument pdfDocument = new iText.Kernel.Pdf.PdfDocument(new PdfReader(stream)))
                                {

                                    // Extract text from each page
                                    for (int pageNumber = 1; pageNumber <= pdfDocument.GetNumberOfPages(); pageNumber++)
                                    {
                                        // Get the page using the page number
                                        iText.Kernel.Pdf.PdfPage page = pdfDocument.GetPage(pageNumber);

                                        // Create a new SimpleTextExtractionStrategy to extract text from the page
                                        LocationTextExtractionStrategy strategy = new LocationTextExtractionStrategy();

                                        // Extract text from the page
                                        string pageText = PdfTextExtractor.GetTextFromPage(page, strategy);

                                        // Append the extracted text from the page to the result
                                        text += pageText;
                                    }
                                }
                            }
                        }
                        catch (Exception)
                        {

                            ModelState.AddModelError("", "Something went wrong while uploading TimeTable. " +
                                "\n Please make sure your have uploaded the correct file.");
                        }

                        var response = await wrapper.Timetable.UploadOrCreate(text);
                        switch ((int)response.StatusCode)
                        {
                            case 200:
                                return RedirectToAction("Display");
                            default:
                                ModelState.AddModelError("", "Something went wrong while uploading TimeTable. " +
                                "\n Please make sure your have uploaded your personal timetable.");
                                break;
                        }
                    }
                    else
                        ModelState.AddModelError("", "Only pdf files allowed");
                }
                else
                    ModelState.AddModelError("", "Chosen file is too large");
            }
            else
            {
                if (startBlank.Count() != 0 && !(await wrapper.Timetable.ClearUserTable(1)).IsSuccessStatusCode)
                    await wrapper.Timetable.UploadOrCreate("");
                else
                    ModelState.AddModelError("", "No file was uploaded. Please upload file or select to continue without uploading.");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ClearTable(int id)
        {
            await wrapper.Timetable.ClearUserTable(id);
            return RedirectToAction("Upload");
        }
        public async Task<ActionResult> Print()
        {
            int latestPeriod = int.Parse(Request.Cookies["latestPeriod"]);

            var response = await wrapper.Session.GetTimetableSessions(Request.Cookies["isForFirstSemester"] == "true");
            switch ((int)response.StatusCode)
            {
                case 200:
                    goto valid;
                case 400:
                    return RedirectToAction("Manage", "Session");
                default:
                    return View("Upload");
            }


        valid:
            Session[,] Model = SessionControlHelpers.Get2DArray(await response.Content.ReadFromJsonAsync<List<Session>>());

            // Create a PDF document
            using (PdfDocument document = new PdfDocument())
            {
                PdfPage page = document.Pages.Add();

                PdfGraphics graphics = page.Graphics;

                RectangleF bounds = new RectangleF(0, 0, document.Pages[0].GetClientSize().Width, 90);

                /*PdfPageTemplateElement header = new PdfPageTemplateElement(bounds);
                header.Graphics.DrawString("B o n g o", new PdfStandardFont(PdfFontFamily.Helvetica, 22, PdfFontStyle.Bold), PdfBrushes.Blue, bounds);

                document.Template.Top = header;*/

                PdfLayoutFormat format = new PdfLayoutFormat()
                {
                    Layout = PdfLayoutType.Paginate,
                    Break = PdfLayoutBreakType.FitPage
                };

                PdfGrid pdfGrid = new PdfGrid();
                pdfGrid.Columns.Add(6);
                pdfGrid.Headers.Add(1);


                PdfStringFormat pdfStringFormat = new PdfStringFormat()
                {
                    Alignment = PdfTextAlignment.Center,
                    LineAlignment = PdfVerticalAlignment.Middle
                };

                PdfGridCellStyle pdfGridCellStyle = new PdfGridCellStyle()
                {
                    Borders = new PdfBorders() { All = PdfPens.Transparent }
                };

                //Headers 
                PdfGridRow pdfGridHeader = pdfGrid.Headers[0];
                pdfGridHeader.ApplyStyle(pdfGridCellStyle);
                pdfGridHeader.Cells[0].Value = "Time";
                pdfGridHeader.Cells[0].Style = new PdfGridCellStyle { Borders = new PdfBorders { Left = PdfPens.Black, Bottom = PdfPens.Blue, Top = PdfPens.Black, Right = PdfPens.Transparent } };
                pdfGridHeader.Cells[1].Value = "Monday";
                pdfGridHeader.Cells[1].Style = new PdfGridCellStyle { Borders = new PdfBorders { Bottom = PdfPens.Blue, Top = PdfPens.Black, Right = PdfPens.Transparent, Left = PdfPens.Transparent } };
                pdfGridHeader.Cells[2].Value = "Tuesday";
                pdfGridHeader.Cells[2].Style = new PdfGridCellStyle { Borders = new PdfBorders { Bottom = PdfPens.Blue, Top = PdfPens.Black, Right = PdfPens.Transparent, Left = PdfPens.Transparent } };
                pdfGridHeader.Cells[3].Value = "Wednesday";
                pdfGridHeader.Cells[3].Style = new PdfGridCellStyle { Borders = new PdfBorders { Bottom = PdfPens.Blue, Top = PdfPens.Black, Right = PdfPens.Transparent, Left = PdfPens.Transparent } };
                pdfGridHeader.Cells[4].Value = "Thursday";
                pdfGridHeader.Cells[4].Style = new PdfGridCellStyle { Borders = new PdfBorders { Bottom = PdfPens.Blue, Top = PdfPens.Black, Right = PdfPens.Transparent, Left = PdfPens.Transparent } };
                pdfGridHeader.Cells[5].Value = "Friday";
                pdfGridHeader.Cells[5].Style = new PdfGridCellStyle { Borders = new PdfBorders { Right = PdfPens.Black, Bottom = PdfPens.Blue, Top = PdfPens.Black, Left = PdfPens.Transparent } };

                for (int i = 1; i < pdfGridHeader.Cells.Count; i++)
                {
                    pdfGridHeader.Cells[i].StringFormat = pdfStringFormat;
                }

                PdfFont headerFont = new PdfStandardFont(PdfFontFamily.Helvetica, 10, PdfFontStyle.Bold);

                pdfGridHeader.Style = new PdfGridRowStyle { Font = headerFont, TextBrush = PdfBrushes.Blue, BackgroundBrush = new PdfSolidBrush(Syncfusion.Drawing.Color.LightGray) };

                //Rows

                pdfGrid.Columns[0].Width = 60f;
                //Handle Rowspan 
                List<List<int>> lstCells = new List<List<int>>();
                for (int i = 0; i < latestPeriod; i++)
                {
                    lstCells.Add(new List<int>());
                }

                // Add table rows
                for (int i = 0; i < latestPeriod; i++)
                {


                    PdfGridRow pdfGridRow = pdfGrid.Rows.Add();
                    pdfGridRow.ApplyStyle(pdfGridCellStyle);
                    PdfGridRowStyle rowStyle = new PdfGridRowStyle { BackgroundBrush = new PdfSolidBrush(i % 2 == 0 ? new PdfColor(ColorTranslator.FromHtml("#dee2e6")) : Syncfusion.Drawing.Color.White) };
                    pdfGridRow.Style = rowStyle;

                    pdfGridRow.Cells[0].Style = new PdfGridCellStyle { Borders = TimetableControlHelpers.GetBorders(latestPeriod, i, -1), Font = headerFont, TextBrush = PdfBrushes.Blue, BackgroundBrush = new PdfSolidBrush(Syncfusion.Drawing.Color.LightGray) };
                    pdfGridRow.Cells[0].Value = GetTime(i);
                    pdfGridRow.Height = 50f;
                    for (int j = 0; j < 5; j++)
                    {

                        Session session = Model[j, i];
                        if (session != null)
                        {
                            int row = SessionControlHelpers.GetInterval(session.sessionInPDFValue);

                            if (row > 1)
                            {
                                for (int k = 0; k < row - 1; k++)
                                {
                                    lstCells[i + k].Add(j);
                                }
                            }
                            if (i > 0 && lstCells[i - 1].Contains(j))
                            {
                                continue;
                            }

                            PdfGrid nestedGrid = new PdfGrid();
                            nestedGrid.Columns.Add();

                            PdfGridRow nestedRow1 = nestedGrid.Rows.Add();
                            nestedRow1.ApplyStyle(new PdfGridCellStyle { Borders = TimetableControlHelpers.GetBorders(latestPeriod, i, j) });
                            nestedRow1.Cells[0].Value = session.ModuleCode;

                            var c = (await (await wrapper.Color.
                                GetModuleColorWithColorDetails(session.ModuleCode)).
                                Content.ReadFromJsonAsync<ModuleColor>()).Color.ColorValue;

                            if (Request.Cookies["isVenueHidden"] == "false")
                            {
                                Regex groupPattern = new Regex(@"\(Group [A-Z]{1,2}[\d]?\)");
                                PdfGridRow nestedRow2 = nestedGrid.Rows.Add();
                                nestedRow2.ApplyStyle(pdfGridCellStyle);
                                nestedRow2.Cells[0].Value = groupPattern.Replace(session.Venue, "");
                                PdfGridCellStyle contentStyle = new PdfGridCellStyle();
                                contentStyle.Borders = new PdfBorders { All = PdfPens.Transparent };
                                contentStyle.CellPadding = new PdfPaddings(0, 0, 0, 5f);
                                contentStyle.TextBrush = new PdfSolidBrush(c == "#FFFFFF" ? new PdfColor(0, 0, 0) : new PdfColor(255, 255, 255));
                                contentStyle.StringFormat = new PdfStringFormat { Alignment = PdfTextAlignment.Center, LineAlignment = PdfVerticalAlignment.Top };
                                nestedRow2.Cells[0].Style = contentStyle;
                                nestedRow1.Height = (float)((50 * row) * (row > 1 ? 0.5 : 0.4));
                                nestedRow2.Height = (float)((50 * row) * (row > 1 ? 0.5 : 0.6));
                                nestedRow1.Cells[0].StringFormat = new PdfStringFormat { Alignment = PdfTextAlignment.Center, LineAlignment = PdfVerticalAlignment.Bottom };
                            }
                            else
                            {
                                nestedRow1.Cells[0].StringFormat = new PdfStringFormat { LineAlignment = PdfVerticalAlignment.Middle, Alignment = PdfTextAlignment.Center };
                                nestedRow1.Height = 50 * row;
                            }

                            PdfGridCellStyle headerStyle = new PdfGridCellStyle();
                            headerStyle.Font = headerFont;
                            headerStyle.StringFormat = pdfStringFormat;

                            nestedRow1.Cells[0].Style = new PdfGridCellStyle { Font = headerFont, Borders = new PdfBorders { All = PdfPens.Transparent }, TextBrush = new PdfSolidBrush(c == "#FFFFFF" ? new PdfColor(0, 0, 0) : new PdfColor(255, 255, 255)) };

                            pdfGridRow.Cells[j + 1].Value = nestedGrid;
                            pdfGridRow.Cells[j + 1].RowSpan = row;



                            Syncfusion.Drawing.Color color = ColorTranslator.FromHtml(c);

                            var fontColor = c == "#FFFFFF" ? new PdfColor(0, 0, 0) : new PdfColor(255, 255, 255);

                            pdfGridRow.Cells[j + 1].Style = new PdfGridCellStyle { Borders = TimetableControlHelpers.GetBorders(latestPeriod, i + row - 1, j), BackgroundBrush = new PdfSolidBrush(new PdfColor(c != "#FFFFFF" ? color : TimetableControlHelpers.GetBackColor(i))) };
                        }
                        else
                        {
                            if (i > 0 && lstCells[i - 1].Contains(j))
                            {
                                continue;
                            }
                            pdfGridRow.Cells[j + 1].Value = "";
                            pdfGridRow.Cells[j + 1].Style = new PdfGridCellStyle { Borders = TimetableControlHelpers.GetBorders(latestPeriod, i, j) };
                        }
                    }

                }

                pdfGrid.Columns[0].Format = pdfStringFormat;
                for (int x = 1; x < pdfGrid.Columns.Count; x++)
                {
                    pdfGrid.Columns[x].Width = 90f;
                }



                pdfGrid.Draw(page, new RectangleF(0, 0, page.GetClientSize().Width, page.GetClientSize().Height), format);

                PdfPageTemplateElement footer = new PdfPageTemplateElement(bounds);

                footer.Graphics.DrawString("\n\n\n\n\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\tBongo | \u00A9 " + DateTime.Now.Year,
                    new PdfStandardFont(PdfFontFamily.Helvetica, 10, PdfFontStyle.Bold), PdfBrushes.LightGray, bounds);

                document.Template.Bottom = footer;

                MemoryStream memoryStream = new MemoryStream();
                document.Save(memoryStream);
                memoryStream.Position = 0;


                return File(memoryStream.ToArray(), "application/pdf", "timetable.pdf");
            }
        }

        [HttpPost]
        public IActionResult AddRow(int latestPeriod)
        {
            return RedirectToAction("Display", new { latestPeriod = latestPeriod });
        }

        [HttpPost]
        public IActionResult UpdateCookie(string key, string value)
        {
            Response.Cookies.Delete(key);
            Response.Cookies.Append(key, value, new CookieOptions() { Expires = DateTime.Now.AddDays(90) });
            return RedirectToAction("Display");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Error()
        {
            return View();
        }

    }
}