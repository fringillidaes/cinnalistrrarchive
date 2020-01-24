﻿using Listrr.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Listrr.Jobs.RecurringJobs
{
    public class GetCountryCodesRecurringJob : IRecurringJob
    {

        private readonly AppDbContext _appDbContext;

        public GetCountryCodesRecurringJob(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }


        public async Task Execute()
        {
            var countries = new List<RegionInfo>();
            foreach (var culture in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
            {
                try
                {
                    var country = new RegionInfo(culture.LCID);
                    if (countries.All(p => p.Name != country.Name))
                        countries.Add(country);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            foreach (var regionInfo in countries)
            {
                if (_appDbContext.CountryCodes.Any(x => x.Code == regionInfo.TwoLetterISORegionName.ToLower())) continue;
                if (regionInfo.TwoLetterISORegionName.Length != 2) continue;

                await _appDbContext.CountryCodes.AddAsync(
                    new CountryCode()
                    {
                        Code = regionInfo.TwoLetterISORegionName.ToLower(),
                        Name = regionInfo.DisplayName
                    }
                );

                await _appDbContext.SaveChangesAsync();
            }


        }
    }
}
