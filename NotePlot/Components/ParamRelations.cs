﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using NotePlot.Models;

namespace NotePlot.Components
{
    public class ParamRelations : ViewComponent
    {
        IRepositoryParameter repo;
        public ParamRelations(IRepositoryParameter r)
        {
            repo = r;
        }

        public IViewComponentResult Invoke(long ParameterID)
        {
            return View("ParamRelationList", repo.GetRelationParameters(ParameterID));
        }

    }
}
