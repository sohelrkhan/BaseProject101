namespace SadaqaAccounting.Shared.EnumConfiguration
{
    public static class BaseEnumConfiguration
    {
        public static class SadaqaAccounting
        {
            public readonly static int GlobalStatus = 1;
            public readonly static int ApplicationUserType = 2;
            public readonly static int PaymentMode = 3;
            public readonly static int TransactionType = 4;
            public readonly static int SourceType = 5;
            public readonly static int Month = 6;
        }
    }

    public static class BaseEnumCollection
    {
        public static class GlobalStatus
        {
            public readonly static int Active = 1;
            public readonly static int Inactive = 2;
            public readonly static int Disposed = 3;
        }

        public static class ApplicationUserType
        {
            public readonly static int Admin = 4;
            public readonly static int Employee = 5;
        }
        
        public static class PaymentMode
        {
            public readonly static int Cash = 6;
            public readonly static int Bank = 7;
        }
        
        public static class TransactionType
        {
            public readonly static int In = 8;
            public readonly static int Out = 9;
        }

        public static class SourceType
        {
            public readonly static int Income = 10;
            public readonly static int Expense = 11;
        }

        public static class Month
        {
            public readonly static int January = 12;
            public readonly static int February = 13;
            public readonly static int March = 14;
            public readonly static int April = 15;
            public readonly static int May = 16;
            public readonly static int June = 17;
            public readonly static int July = 18;
            public readonly static int August = 19;
            public readonly static int September = 20;
            public readonly static int Octobor = 21;
            public readonly static int November = 22;
            public readonly static int December = 23;
        }
    }
}