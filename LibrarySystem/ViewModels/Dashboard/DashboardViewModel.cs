namespace LibrarySystem.web.ViewModels.Dashboard;

    public class DashboardViewModel
    {
        // Stat Cards
        public int TotalBooks { get; set; }
        public int TotalMembers { get; set; }
        public int ActiveLoans { get; set; }
        public int OverdueLoans { get; set; }
        public int PendingReservations { get; set; }
        public decimal UnpaidFinesTotal { get; set; }

        // أكتر 5 كتب استعارة
        public List<string> TopBooksLabels { get; set; } = new();
        public List<int> TopBooksCounts { get; set; } = new();

        // الاستعارات آخر 14 يوم
        public List<string> LoansTimelineLabels { get; set; } = new();
        public List<int> LoansTimelineCounts { get; set; } = new();

        // توزيع الكتب على التصنيفات
        public List<string> CategoryLabels { get; set; } = new();
        public List<int> CategoryCounts { get; set; } = new();
    }

