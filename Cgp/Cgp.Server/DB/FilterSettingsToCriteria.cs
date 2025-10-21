using System;
using System.Collections;
using Contal.Cgp.Server.Beans;
using NHibernate;
using NHibernate.Criterion;
using Contal.Cgp.BaseLib;

namespace Contal.Cgp.Server.DB
{
    public static class FilterSettingsToCriteria
    {
        public static ICriteria AddCriteria(ICriteria criteria, FilterSettings filterSettings)
        {
            if (filterSettings == null)
                return criteria;

            if (filterSettings.Value == null)
            {
                return
                    criteria.Add(
                        filterSettings.ComparerMode == ComparerModes.EQUALL
                            ? Restrictions.IsNull(filterSettings.Column)
                            : Restrictions.Not(Restrictions.IsNull(filterSettings.Column)));
            }
            if (filterSettings.Value is DateTime)
            {
                DateTime dateTime = (filterSettings.Value as DateTime?).Value;

                switch (filterSettings.ComparerMode)
                {
                    case ComparerModes.EQUALL:
                    {
                        DateTime startDateTime = dateTime.Date.AddMilliseconds(-1);
                        DateTime endDateTime = startDateTime.AddHours(24);

                        return 
                            criteria.Add(
                                Restrictions.Between(
                                    filterSettings.Column,
                                    startDateTime,
                                    endDateTime));
                    }
                    case ComparerModes.MORE:

                        return 
                            criteria.Add(
                                Restrictions.Gt(
                                    filterSettings.Column, 
                                    dateTime));

                    case ComparerModes.EQUALLMORE:

                        return 
                            criteria.Add(
                                Restrictions.Gt(
                                    filterSettings.Column,
                                    dateTime.AddMilliseconds(-1)));

                    case ComparerModes.LESS:

                        return 
                            criteria.Add(
                                Restrictions.Or(
                                    Restrictions.IsNull(filterSettings.Column),
                                    Restrictions.Lt(filterSettings.Column, dateTime)));

                    case ComparerModes.EQUALLLESS:

                        return 
                            criteria.Add(
                                Restrictions.Or(
                                    Restrictions.IsNull(filterSettings.Column),
                                    Restrictions.Lt(
                                        filterSettings.Column,
                                        dateTime.AddMilliseconds(1))));
                }

                return criteria;
            }

            switch (filterSettings.ComparerMode)
            {
                case ComparerModes.EQUALL:

                    return 
                        criteria.Add(
                            Restrictions.Eq(
                                filterSettings.Column, 
                                filterSettings.Value));

                case ComparerModes.MORE:

                    return 
                        criteria.Add(
                            Restrictions.Gt(
                                filterSettings.Column, 
                                filterSettings.Value));

                case ComparerModes.LESS:

                    return 
                        criteria.Add(
                            Restrictions.Lt(
                                filterSettings.Column, 
                                filterSettings.Value));

                case ComparerModes.LIKE:

                    return 
                        criteria.Add(
                            Restrictions.Like(
                                filterSettings.Column,
                                filterSettings.Value as string,
                                MatchMode.Start));

                case ComparerModes.LIKEBOTH:

                    return 
                        criteria.Add(
                            Restrictions.Like(
                                filterSettings.Column,
                                filterSettings.Value as string,
                                MatchMode.Anywhere));

                case ComparerModes.NOTEQUALL:

                    return
                        criteria.Add(
                            Restrictions.Not(
                                Restrictions.Eq(
                                    filterSettings.Column, 
                                    filterSettings.Value)));

                case ComparerModes.IN:

                    return
                        criteria.Add(
                            Restrictions.In(
                                filterSettings.Column,
                                filterSettings.Value as ICollection));
            }

            return criteria;
        }

        public static Junction AddJunction(Junction junction, FilterSettings filterSettings)
        {
            if (filterSettings == null)
                return junction;

            if (filterSettings.Value == null)
            {
                switch (filterSettings.ComparerMode)
                {
                    case ComparerModes.EQUALL:

                        return
                            junction.Add(
                                Restrictions.IsNull(filterSettings.Column));
                    default:

                        return
                            junction.Add(
                                Restrictions.Not(
                                    Restrictions.IsNull(filterSettings.Column)));
                }
            }

            if (filterSettings.Value is DateTime)
            {
                DateTime dateTime = (filterSettings.Value as DateTime?).Value;

                switch (filterSettings.ComparerMode)
                {
                    case ComparerModes.EQUALL:
                    {
                        DateTime startDateTime = dateTime.Date.AddMilliseconds(-1);
                        DateTime endDateTime = startDateTime.AddHours(24);

                        return
                            junction.Add(
                                Restrictions.Between(
                                    filterSettings.Column,
                                    startDateTime,
                                    endDateTime));
                    }

                    case ComparerModes.MORE:

                        return
                            junction.Add(
                                Restrictions.Gt(
                                    filterSettings.Column, 
                                    dateTime));

                    case ComparerModes.EQUALLMORE:

                        return
                            junction.Add(
                                Restrictions.Gt(
                                    filterSettings.Column,
                                    dateTime.AddMilliseconds(-1)));

                    case ComparerModes.LESS:

                        return
                            junction.Add(
                                Restrictions.Or(
                                    Restrictions.IsNull(filterSettings.Column),
                                    Restrictions.Lt(filterSettings.Column, dateTime)));

                    case ComparerModes.EQUALLLESS:

                        return
                            junction.Add(
                                Restrictions.Or(
                                    Restrictions.IsNull(filterSettings.Column),
                                    Restrictions.Lt(
                                        filterSettings.Column,
                                        dateTime.AddMilliseconds(1))));
                }

                return junction;
            }

            switch (filterSettings.ComparerMode)
            {
                case ComparerModes.EQUALL:

                    return
                        junction.Add(
                            Restrictions.Eq(
                                filterSettings.Column,
                                filterSettings.Value));

                case ComparerModes.MORE:

                    return
                        junction.Add(
                            Restrictions.Gt(
                                filterSettings.Column, 
                                filterSettings.Value));

                case ComparerModes.LESS:

                    return
                        junction.Add(
                            Restrictions.Lt(
                                filterSettings.Column, 
                                filterSettings.Value));

                case ComparerModes.LIKE:

                    return
                        junction.Add(
                            Restrictions.Like(
                                filterSettings.Column,
                                filterSettings.Value as string,
                                MatchMode.Start));

                case ComparerModes.LIKEBOTH:

                    return
                        junction.Add(
                            Restrictions.Like(
                                filterSettings.Column,
                                filterSettings.Value as string,
                                MatchMode.Anywhere));

                case ComparerModes.NOTEQUALL:

                    return
                        junction.Add(
                            Restrictions.Not(
                                Restrictions.Eq(filterSettings.Column, filterSettings.Value)));

                case ComparerModes.IN:

                    return
                        junction.Add(
                            Restrictions.In(
                                filterSettings.Column,
                                filterSettings.Value as ICollection));
            }

            return junction;
        }
    }
}
