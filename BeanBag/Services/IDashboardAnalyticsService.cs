﻿using System.Linq;

namespace BeanBag.Services
{
    public interface IDashboardAnalyticsService
    {
        public IOrderedQueryable GetRecentItems(string id);


    }
}