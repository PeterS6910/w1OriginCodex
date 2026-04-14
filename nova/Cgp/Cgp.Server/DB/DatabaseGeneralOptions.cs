using System;

using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.Server.DB
{
    public sealed class DatabaseGeneralOptions :
        ABaseOrmTable<DatabaseGeneralOptions, DatabaseGeneralOption>
    {
        private DatabaseGeneralOptions() : base(null)
        {
        }

        public override object ParseId(string strObjectId)
        {
            int result;

            return
                int.TryParse(strObjectId, out result)
                    ? (object)result
                    : null;
        }

        public override bool HasAccessView(Login login)
        {
            return AccessChecker.HasAccessControl(BaseAccess.GetAccessesForGroup(LoginAccessGroups.GENERAL_OPTIONS), login);
        }

        public override bool HasAccessInsert(Login login)
        {
            return AccessChecker.HasAccessControl(BaseAccess.GetAccessesForGroup(LoginAccessGroups.GENERAL_OPTIONS), login);
        }

        public override bool HasAccessUpdate(Login login)
        {
            return AccessChecker.HasAccessControl(BaseAccess.GetAccessesForGroup(LoginAccessGroups.GENERAL_OPTIONS), login);
        }

        public override bool HasAccessDelete(Login login)
        {
            return AccessChecker.HasAccessControl(BaseAccess.GetAccessesForGroup(LoginAccessGroups.GENERAL_OPTIONS), login);
        }

        /// <summary>
        /// Insert the value to the database. If insert succesed return true, otherwise false.
        /// </summary>
        /// <param name="databaseGeneralOptionType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Set(DatabaseGeneralOptionType databaseGeneralOptionType, bool value)
        {
            try
            {
                // Delete old value from the database
                var databaseGeneralOption = GetById((int)databaseGeneralOptionType);
                if (databaseGeneralOption != null)
                {
                    if (!Delete(databaseGeneralOption))
                        return false;
                }

                // Save new value to the database
                var newDatabaseGeneralOption = new DatabaseGeneralOption
                {
                    IdDatabaseGeneralOption = (int)databaseGeneralOptionType,
                    IntValue = value ? 1 : 0
                };

                return Insert(ref newDatabaseGeneralOption);
            }
            catch { }

            return false;
        }

        /// <summary>
        /// Insert the value to the database. If insert succesed return true, otherwise false.
        /// </summary>
        /// <param name="databaseGeneralOptionType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Set(DatabaseGeneralOptionType databaseGeneralOptionType, bool? value)
        {
            try
            {
                var databaseGeneralOption = GetById((int)databaseGeneralOptionType);
                if (databaseGeneralOption != null)
                {
                    if (!Delete(databaseGeneralOption))
                        return false;
                }

                var newDatabaseGeneralOption = new DatabaseGeneralOption
                {
                    IdDatabaseGeneralOption = (int)databaseGeneralOptionType
                };

                if (value == null)
                {
                    newDatabaseGeneralOption.IntValue = null;
                }
                else
                {
                    newDatabaseGeneralOption.IntValue = value.Value ? 1 : 0;
                }

                return Insert(ref newDatabaseGeneralOption);
            }
            catch { }

            return false;
        }

        /// <summary>
        /// Insert the value to the database. If insert succesed return true, otherwise false.
        /// </summary>
        /// <param name="databaseGeneralOptionType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Set(DatabaseGeneralOptionType databaseGeneralOptionType, int value)
        {
            try
            {
                var databaseGeneralOption = GetById((int)databaseGeneralOptionType);
                if (databaseGeneralOption != null)
                {
                    if (!Delete(databaseGeneralOption))
                        return false;
                }

                var newDatabaseGeneralOption = new DatabaseGeneralOption
                {
                    IdDatabaseGeneralOption = (int)databaseGeneralOptionType,
                    IntValue = value
                };
                return Insert(ref newDatabaseGeneralOption);
            }
            catch { }

            return false;
        }

        /// <summary>
        /// Insert the value to the database. If insert succesed return true, otherwise false.
        /// </summary>
        /// <param name="databaseGeneralOptionType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Set(DatabaseGeneralOptionType databaseGeneralOptionType, int? value)
        {
            try
            {
                var databaseGeneralOption = GetById((int)databaseGeneralOptionType);
                if (databaseGeneralOption != null)
                {
                    if (!Delete(databaseGeneralOption))
                        return false;
                }

                var newDatabaseGeneralOption = new DatabaseGeneralOption
                {
                    IdDatabaseGeneralOption = (int)databaseGeneralOptionType,
                    IntValue = value
                };
                return Insert(ref newDatabaseGeneralOption);
            }
            catch { }

            return false;
        }

        /// <summary>
        /// Insert the value to the database. If insert succesed return true, otherwise false.
        /// </summary>
        /// <param name="databaseGeneralOptionType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Set(DatabaseGeneralOptionType databaseGeneralOptionType, string value)
        {
            try
            {
                var databaseGeneralOption = GetById((int)databaseGeneralOptionType);
                if (databaseGeneralOption != null)
                {
                    if (!Delete(databaseGeneralOption))
                        return false;
                }

                var newDatabaseGeneralOption = new DatabaseGeneralOption
                {
                    IdDatabaseGeneralOption = (int)databaseGeneralOptionType,
                    StringValue = value
                };
                return Insert(ref newDatabaseGeneralOption);
            }
            catch { }

            return false;
        }

        /// <summary>
        /// Insert the value to the database. If insert succesed return true, otherwise false.
        /// </summary>
        /// <param name="databaseGeneralOptionType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Set(DatabaseGeneralOptionType databaseGeneralOptionType, Guid value)
        {
            try
            {
                var databaseGeneralOption = GetById((int)databaseGeneralOptionType);
                if (databaseGeneralOption != null)
                {
                    if (!Delete(databaseGeneralOption))
                        return false;
                }

                var newDatabaseGeneralOption = new DatabaseGeneralOption
                {
                    IdDatabaseGeneralOption = (int)databaseGeneralOptionType,
                    StringValue = value.ToString()
                };
                return Insert(ref newDatabaseGeneralOption);
            }
            catch { }

            return false;
        }

        /// <summary>
        /// Read the value from the database. If this general option type is not in the database return false.
        /// </summary>
        /// <param name="databaseGeneralOptionType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Get(DatabaseGeneralOptionType databaseGeneralOptionType, out bool value)
        {
            try
            {
                var databaseGeneralOption = GetById((int)databaseGeneralOptionType);

                // Set value and return true only if database value is not null
                if (databaseGeneralOption != null && databaseGeneralOption.IntValue != null)
                {
                    value = databaseGeneralOption.IntValue.Value != 0;
                    return true;
                }
            }
            catch { }

            value = false;
            return false;
        }

        /// <summary>
        /// Read the value from the database. If this general option type is not in the database return false.
        /// </summary>
        /// <param name="databaseGeneralOptionType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Get(DatabaseGeneralOptionType databaseGeneralOptionType, out bool? value)
        {
            try
            {
                var databaseGeneralOption = GetById((int)databaseGeneralOptionType);
                if (databaseGeneralOption != null)
                {
                    if (databaseGeneralOption.IntValue == null)
                        value = null;
                    else
                        value = databaseGeneralOption.IntValue.Value != 0;
                    return true;
                }
            }
            catch { }

            value = false;
            return false;
        }

        /// <summary>
        /// Read the value from the database. If this general option type is not in the database return false.
        /// </summary>
        /// <param name="databaseGeneralOptionType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Get(DatabaseGeneralOptionType databaseGeneralOptionType, out int value)
        {
            try
            {
                var databaseGeneralOption = GetById((int)databaseGeneralOptionType);

                // Set value and return true only if database value is not null
                if (databaseGeneralOption != null && databaseGeneralOption.IntValue != null)
                {
                    value = databaseGeneralOption.IntValue.Value;
                    return true;
                }
            }
            catch { }

            value = 0;
            return false;
        }

        /// <summary>
        /// Read the value from the database. If this general option type is not in the database return false.
        /// </summary>
        /// <param name="databaseGeneralOptionType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Get(DatabaseGeneralOptionType databaseGeneralOptionType, out int? value)
        {
            try
            {
                var databaseGeneralOption = GetById((int)databaseGeneralOptionType);
                if (databaseGeneralOption != null)
                {
                    value = databaseGeneralOption.IntValue;
                    return true;
                }
            }
            catch { }

            value = 0;
            return false;
        }

        /// <summary>
        /// Read the value from the database. If this general option type is not in the database return false.
        /// </summary>
        /// <param name="databaseGeneralOptionType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Get(DatabaseGeneralOptionType databaseGeneralOptionType, out string value)
        {
            try
            {
                var databaseGeneralOption = GetById((int)databaseGeneralOptionType);
                if (databaseGeneralOption != null)
                {
                    value = databaseGeneralOption.StringValue;
                    return true;
                }
            }
            catch { }

            value = string.Empty;
            return false;
        }

        /// <summary>
        /// Read the value from the database. If this general option type is not in the database return false.
        /// </summary>
        /// <param name="databaseGeneralOptionType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Get(DatabaseGeneralOptionType databaseGeneralOptionType, out Guid value)
        {
            try
            {
                var databaseGeneralOption = GetById((int)databaseGeneralOptionType);
                if (databaseGeneralOption != null)
                {
                    value = new Guid(databaseGeneralOption.StringValue);
                    return true;
                }
            }
            catch { }

            value = Guid.Empty;
            return false;
        }

        /// <summary>
        /// Return true if values from registry have already been saved into the database, otherwise false
        /// </summary>
        /// <returns></returns>
        public bool GetSavedValuesFromRegistryToDatabase()
        {
            try
            {
                var databaseGeneralOption = GetById((int)DatabaseGeneralOptionType.SavedValuesFromRegistryToDatabase);
                if (databaseGeneralOption != null)
                {
                    return true;
                }
            }
            catch { }

            return false;
        }

        /// <summary>
        /// Mark in the database, that the values from registry have already been saved into the database
        /// </summary>
        public void SetSavedValuesFromRegistryToDatabase()
        {
            try
            {
                var databaseGeneralOption = GetById((int)DatabaseGeneralOptionType.SavedValuesFromRegistryToDatabase);
                if (databaseGeneralOption == null)
                {
                    var newDatabaseGeneralOption = new DatabaseGeneralOption
                    {
                        IdDatabaseGeneralOption =
                            (int)DatabaseGeneralOptionType.SavedValuesFromRegistryToDatabase
                    };
                    Insert(ref newDatabaseGeneralOption);
                }
            }
            catch { }
        }

        public override Contal.Cgp.Globals.ObjectType ObjectType
        {
            get { return Contal.Cgp.Globals.ObjectType.ServerGeneralOptionsDB; }
        }
    }
}
