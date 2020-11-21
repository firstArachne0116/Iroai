using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace SmartApp
{
    public interface ISQLite
    {
        SQLiteAsyncConnection GetAsyncConnection();
    }
}
