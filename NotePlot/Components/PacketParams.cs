using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NotePlot.Models;

namespace NotePlot.Components
{
    public class PacketParams : ViewComponent
    {
        IRepositoryParameter repo;
        public PacketParams(IRepositoryParameter r)
        {
            repo = r;
        }

        public IViewComponentResult Invoke(long? PacketID, long LoginID)
        {
            ViewBag.LoginID = LoginID;
            return View("PacketParamList", repo.GetPacketParameters(PacketID));
        }
    }
}