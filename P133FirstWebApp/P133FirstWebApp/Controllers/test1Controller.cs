using System.Xml.Linq;

namespace P133FirstWebApp.Controllers
{
    public class test1Controller
    {

        public string test1Method(int a, string name,int id)
        {
            return name+" Hello P113 "+a+" "+id;
        }
        public string test2Method(int a, string name)
        {
            return name + " Hello P113 " + a;
        }
        public string test3Method(int a)
        {
            return " Hello P113 " + a;
        }
    }
}
