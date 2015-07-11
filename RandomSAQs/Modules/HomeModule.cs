using Nancy;

namespace RandomSAQs.Modules
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get["/"] = _ => View["Index"];
            Get["/about"] = _ => View["About"];
        }
    }
}