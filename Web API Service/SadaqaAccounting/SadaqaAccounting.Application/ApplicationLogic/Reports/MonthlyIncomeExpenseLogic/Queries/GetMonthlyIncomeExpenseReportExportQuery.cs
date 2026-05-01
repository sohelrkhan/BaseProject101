using SadaqaAccounting.Application.ApplicationLogic.Reports.MonthlyIncomeExpenseLogic.Model;

namespace SadaqaAccounting.Application.ApplicationLogic.Reports.MonthlyIncomeExpenseLogic.Queries
{
    public class GetMonthlyIncomeExpenseReportExportQuery: ExportMonthlyIncomeExpenseReportModel,IRequest<byte[]>
    {
        public class Handler : IRequestHandler<GetMonthlyIncomeExpenseReportExportQuery, byte[]>
        {
            private readonly IWebHostEnvironment _webHostEnvironment;
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly ICompanyRepository _companyRepository;
            private readonly IImageService _imageService;
            private readonly IExpenseRepository _expenseRepository;
            private readonly IIncomeRepository _incomeRepository;
            private readonly IEventRepository _eventRepository;

            public Handler(IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor, ICompanyRepository companyRepository,
                IImageService imageService, IExpenseRepository expenseRepository, IIncomeRepository incomeRepository, IEventRepository eventRepository)
            {
                _webHostEnvironment = webHostEnvironment;
                _httpContextAccessor = httpContextAccessor;
                _companyRepository = companyRepository;
                _imageService = imageService;
                _expenseRepository = expenseRepository;
                _incomeRepository = incomeRepository;
                _eventRepository = eventRepository;
            }

            public async Task<byte[]> Handle(GetMonthlyIncomeExpenseReportExportQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                // Retrieve the user's accountUnitId from the current HTTP context
                var AccountUnitId = int.Parse(_httpContextAccessor.HttpContext?.User?.FindFirst("AccountUnitId")?.Value!);

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                var user = _httpContextAccessor.HttpContext?.User;

                string? createdByUserfullName = user?.FindFirst("FullName")?.Value;

                var propertyList = new List<MonthlyIncomeExpenseSummaryModel>();
                var propertyListForExcel = new List<MonthlyIncomeExpenseExcelModel>();

                int incomeSl = 0;
                int expenseSl = 0;
                // from income list
                foreach (var item in request.GridModel.IncomeList)
                {
                    var model = new MonthlyIncomeExpenseSummaryModel
                    {
                        SL = incomeSl + 1,
                        SourceType = "Income",
                        IncomeCategoryName = item.IncomeCategoryName,
                        ReceiptDate = item.ReceiptDate,
                        IncomeAmount = item.Amount
                    };

                    propertyList.Add(model);
                }
                // from income list
                foreach (var item in request.GridModel.IncomeList)
                {
                    var model = new MonthlyIncomeExpenseExcelModel
                    {
                        SL = incomeSl + 1,
                        SourceType = "Income",
                        IncomeCategoryName = item.IncomeCategoryName,
                        ReceiptDate = item.ReceiptDate,
                        IncomeAmount = item.Amount
                    };

                    propertyListForExcel.Add(model);
                }
                // from expense list
                foreach (var item in request.GridModel.ExpenseList)
                {
                    var model = new MonthlyIncomeExpenseSummaryModel
                    {
                        SL = expenseSl + 1,
                        SourceType = "Expense",
                        ExpenseCategoryName = item.ExpenseCategoryName,
                        ExpenseDate = item.ExpenseDate,
                        ExpenseAmount = item.Amount
                    };
                    propertyList.Add(model);
                }
                // from expense list
                foreach (var item in request.GridModel.ExpenseList)
                {
                    var model = new MonthlyIncomeExpenseExcelModel
                    {
                        SL = expenseSl + 1,
                        SourceType = "Expense",
                        ExpenseCategoryName = item.ExpenseCategoryName,
                        ExpenseDate = item.ExpenseDate,
                        ExpenseAmount = item.Amount
                    };
                    propertyListForExcel.Add(model);
                }

                // Retrieve the user's Company Id from the current HTTP context
                //var companyId = _httpContextAccessor.HttpContext?.User?.FindFirst("CompanyId")?.Value;

                //if (companyId is null)
                //{
                //    throw new Exception(ProvideErrorMessage.CompanyIdNotFound);
                //}

                //request.CompanyId = Convert.ToInt32(companyId);
                var getAllCompany = await _companyRepository.GetAllAsync();
                var getCompany = getAllCompany.FirstOrDefault();
                byte[] logoImage = _imageService.GetCompanyLogo(getCompany.Logo);



                using var qrGenerator = new QRCodeGenerator();
                using var qrCodeData = qrGenerator.CreateQrCode(request.QrCodeUrl, QRCodeGenerator.ECCLevel.Q);
                using var qrCode = new PngByteQRCode(qrCodeData);
                byte[] qrCodeImage = qrCode.GetGraphic(20);
                string EventName = "";

                if (request.EventId != null) {
                    var eventDetails = await _eventRepository.GetByIdAsync(Convert.ToInt32(request.EventId));
                    if (eventDetails != null) {
                        EventName = eventDetails.Name;
                    }
                }
                

                var filters = new List<ReportFilter>();
                filters.Add(new ReportFilter
                {
                    Key = "Duration",
                    Value = request.Duration!
                });
                filters.Add(new ReportFilter
                {
                    Key = "Event",
                    Value = EventName!
                });

                var incomeHeaders = new Dictionary<string, string>
                {
                    { "IncomeCategoryName", "Source" },
                    { "ReceiptDate", "Date" },
                    { "IncomeAmount", "Amount" }
                };

                var expenseHeaders = new Dictionary<string, string>
                {
                    { "ExpenseCategoryName", "Purpose" },
                    { "ExpenseDate", "Date" },
                    { "ExpenseAmount", "Amount" }
                };

               
               

                if (request.ReportType == "pdf")
                {
                    return new StandardIncomeExpensePdfService<MonthlyIncomeExpenseSummaryModel, ReportFilter>(incomeHeaders, expenseHeaders, true)
                           .GeneratePDF(request.ReportName,
                           getCompany.Name, getCompany.Address, getCompany.Email, getCompany.Website, getCompany.Mobile,
                           createdByUserfullName, filters, propertyList, logoImage, qrCodeImage);
                }
                else
                {
                    return new StandardIncomeExpenseExcelService<MonthlyIncomeExpenseExcelModel, ReportFilter>(incomeHeaders, expenseHeaders, true)
                           .ExportToExcel(request.ReportName,
                           getCompany.Name, getCompany.Address, getCompany.Email, getCompany.Website, getCompany.Mobile,
                           createdByUserfullName, filters, propertyListForExcel, logoImage, qrCodeImage);
                }
            }
        }
    }
}
