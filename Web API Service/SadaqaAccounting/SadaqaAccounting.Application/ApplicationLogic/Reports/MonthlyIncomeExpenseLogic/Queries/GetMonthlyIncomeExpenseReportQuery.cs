using SadaqaAccounting.Application.ApplicationLogic.ExpenseManagement.ExpenseLogic.Queries;
using SadaqaAccounting.Application.ApplicationLogic.IncomeManagement.IncomeLogic.Model;
using SadaqaAccounting.Application.ApplicationLogic.Reports.MonthlyIncomeExpenseLogic.Model;

namespace SadaqaAccounting.Application.ApplicationLogic.Reports.MonthlyIncomeExpenseLogic.Queries
{
    public class GetMonthlyIncomeExpenseReportQuery: MonthlyIncomeExpenseRequestModel,IRequest<MonthlyIncomeExpenseGridModel>
    {
        public class Handler : IRequestHandler<GetMonthlyIncomeExpenseReportQuery, MonthlyIncomeExpenseGridModel>
        {
            private readonly IExpenseRepository _expenseRepository;
            private readonly IIncomeRepository _incomeRepository;
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IExpenseRepository expenseRepository, IIncomeRepository incomeRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            {
                _expenseRepository = expenseRepository;
                _incomeRepository = incomeRepository;
                _mapper = mapper;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<MonthlyIncomeExpenseGridModel> Handle(GetMonthlyIncomeExpenseReportQuery request, CancellationToken cancellationToken)
            {
                // Retrieve the user's Id from the current HTTP context
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                var userName = _httpContextAccessor.HttpContext?.User?.FindFirst("FullName")?.Value;

                // Retrieve the user's accountUnitId from the current HTTP context
                var AccountUnitId = int.Parse(_httpContextAccessor.HttpContext?.User?.FindFirst("AccountUnitId")?.Value!);

                // Check if the user Id is null or not
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException(ProvideErrorMessage.UserNotAuthenticated);

                // get all expense during this period
                var getAllExpense = await _expenseRepository.GetExpenseSummaryByFilterAsync(AccountUnitId,request.EventId,Convert.ToDateTime(request.FromDate),Convert.ToDateTime(request.ToDate));
                var mapAllExpense = _mapper.Map<ICollection<ExpenseGridModel>>(getAllExpense);

                // get all income during this period
                var getAllIncome = await _incomeRepository.GetIncomeSummaryByFilterAsync(AccountUnitId, request.EventId, Convert.ToDateTime(request.FromDate), Convert.ToDateTime(request.ToDate));
                var mapAllIncome = _mapper.Map<ICollection<IncomeGridModel>>(getAllIncome);

                var reponseModel = new MonthlyIncomeExpenseGridModel
                {
                    PreparedByName = userName!,
                    Period = request.DateRange,
                    IncomeList = mapAllIncome,
                    ExpenseList = mapAllExpense,
                    TotalIncome = mapAllIncome.Sum(s => s.Amount),
                    TotalExpense = mapAllExpense.Sum(s => s.Amount),
                };

                return reponseModel;
            }
        }
    }
}
