using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Next_Level.Classes
{
    public static class NextLevelPath
    {
        public static string ACCOUNT_PATH { get; } = @"..\Accounts.bin";
        public static string PRODUCT_PATH { get; } = @"..\Products.xml";
        public static string CURRENT_USER { get; } = @"..\CurrentLogin.bin";
        public static string SAVE_LOGIN { get; } = @"..\SaveLogin.bin";
        //public static string PROJECT_PATH { get; } = @"..\Debug";
        public static string STOREBD_PATH { get; } = @"..\bd";
        public static string CATEGORIES_PATH { get; } = @"..\Categories.xml";
        public static string CART_PATH { get; }= @"..\Cart.xml";
        public static readonly String ConnectionString = @"
            Data Source=(LocalDB)\MSSQLLocalDB;
            AttachDbFilename=C:\Users\dsgnrr\source\repos\Next Level\Next Level\DataBase\NextLevelDB.mdf;
            Integrated Security=True";
    }
}
