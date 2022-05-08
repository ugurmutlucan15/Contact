using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ReportMicroService.Entities;

namespace ReportMicroService.Repositories.Interfaces
{
    public interface IReportRepository
    { 
        Task<IEnumerable<Report>> GetReports(Expression<Func<Report,bool>> filter=null);
        Task<Report> GetReport(string id);
        Task<Report> Create(Report model);
        Task<bool> Update(Report model);
        Task<bool> Delete(string id);
    }
}