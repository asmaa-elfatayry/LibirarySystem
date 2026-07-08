

namespace Domain.Enums;

public enum LoanStatus
{
    Active,      // الاستعارة شغالة، لسه ماترجعتش
        Returned,    // اترجعت في الميعاد أو بعده
        Overdue      // فات ميعاد الإرجاع ولسه ماترجعتش
}
