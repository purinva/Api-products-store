namespace Api.Common
{
    public static class SharedData
    {
        public static class Roles
        {
            public const string Admin = "admin";
            public const string Consumer = "consumer";

            public static IReadOnlyList<string> AllRoles
            {
                get => new List<string>() { Admin, Consumer };
            }
        }

        public static class OrderStatus
        {
            public const string Pending = "pending";
            public const string ReadyToShip = "ready_to_ship";
            public const string Completed = "completed";

            public static IReadOnlyList<string> AllStatus
            {
                get => new List<string>() { Pending, ReadyToShip, Completed };
            }
        }
    }
}
