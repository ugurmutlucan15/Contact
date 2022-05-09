using MongoDB.Driver;

using ReportMicroService.Data.Interfaces;
using ReportMicroService.Entities;
using ReportMicroService.Repositories.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ReportMicroService.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly IReportContext _ctx;

        public ReportRepository(IReportContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<IEnumerable<Report>> GetReports(Expression<Func<Report, bool>> filter = null)
        {
            if (filter != null)
            {
                return await _ctx.Report.Find(filter).ToListAsync();
            }

            return await _ctx.Report.Find(m => true).ToListAsync();
        }

        public async Task<Report> GetReport(string id)
        {
            return await _ctx.Report.Find(m => m.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Report> Create(Report model)
        {
            await _ctx.Report.InsertOneAsync(model);
            return model;
        }

        public async Task<bool> Update(Report model)
        {
            var updateResult = await _ctx.Report.ReplaceOneAsync(filter: g => g.Id == model.Id, replacement: model);
            return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
        }

        public async Task<bool> Delete(string id)
        {
            var filter = Builders<Report>.Filter.Eq(m => m.Id, id);
            var deleteResult = await _ctx.Report.DeleteOneAsync(filter);
            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }
    }
}