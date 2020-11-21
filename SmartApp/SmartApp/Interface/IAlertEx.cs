using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartApp.Interface
{
    public interface IAlertEx
    {
        Task AlertMsg(string msg, string title, string actionName1, string actionName2, string actionNameCancel, Func<int, Task> action1, Func<int, Task> action2);
    }

}
