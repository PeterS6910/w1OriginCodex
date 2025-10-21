using System;
using System.Collections.Generic;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.IwQuick;
using Contal.Cgp.NCAS.Server.DB;
using Contal.IwQuick.Crypto;
using Contal.IwQuick.Data;
using Contal.IwQuick.Remoting;
using Contal.IwQuick.Threads;

namespace Contal.Cgp.NCAS.Server
{
    public sealed class ComboLicenceManager : ASingleton<ComboLicenceManager>
    {
        private static readonly byte[] COMBO_LICENCE_KEY = { 0x5E, 0x9B, 0x3A, 0x76, 0x11, 0x00, 0x2D, 0x35, 0x47, 0x20, 0x6B, 0xC8, 0x28, 0x8F, 0xAA, 0xEA };
        private static readonly byte[] COMBO_LICENCE_SALT = { 0x01, 0x59, 0x55, 0x5A, 0x4D, 0x11, 0xFB, 0x14, 0x77, 0xC7, 0xA1, 0x51, 0x59, 0x2D, 0xDB, 0x22 };

        private readonly HashSet<Guid> _ccuWithLicences = new HashSet<Guid>();
        private readonly Random _random = new Random();

        private readonly ProcessingQueue<int> _eventInvoker = new ProcessingQueue<int>(); 

        private ComboLicenceManager()
            : base(null)
        {
            _eventInvoker.ItemProcessing += InvokeEvent;

            var ccus = CCUs.Singleton.List();

            if (ccus == null) 
                return;

            int licenceCount = LicenceCount;
            lock (_ccuWithLicences)
            {
                foreach (var ccu in ccus)
                {
                    //TODO uncomment this when mainboard type will be present in DB
                    //if (ccu.CcuMainboardType != (byte)MainBoardVariant.CAT12CE)
                    //    continue;

                    if (ccu.Cat12ComboLicence == null
                        || ccu.Cat12ComboLicence.Length != 32)
                    {
                        UpdateLicenceInDB(ccu.IdCCU, true);
                        _ccuWithLicences.Add(ccu.IdCCU);
                    }
                    else
                    {
                        if (!HasLicenceInDB(ccu.IdCCU, ccu.Cat12ComboLicence)) 
                            continue;

                        if (_ccuWithLicences.Count < licenceCount)
                        {
                            _ccuWithLicences.Add(ccu.IdCCU);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guidCcu"></param>
        /// <returns></returns>
        public byte [] GetInitialValueForDb(Guid guidCcu)
        {
            try
            {
                return GetValueForDb(guidCcu, false);
            }
            catch
            {
                return null;
            }
        }

        private byte[] GetValueForDb(Guid guidCcu, bool hasLicence)
        {
            byte[] rawData = new byte[24];
            Array.Copy(guidCcu.ToByteArray(), 0, rawData, 2, 16);
            rawData[1] = (byte)_random.Next(255);
            rawData[18] = (byte)_random.Next(255);
            rawData[0] = (byte)(hasLicence ? 1 : 0);

            var tempCrc = BitConverter.GetBytes(
                Crc32.ComputeChecksum(rawData, 0, rawData.Length - 4));

            Array.Copy(tempCrc, 0, rawData, rawData.Length - 4, 4);

            return QuickCrypto.Encrypt(
                rawData,
                COMBO_LICENCE_KEY,
                COMBO_LICENCE_SALT);
        }

        private void UpdateLicenceInDB(Guid guidCcu, bool hasLicence)
        {
            try
            {
                byte[] data = GetValueForDb(guidCcu, hasLicence);

                // Save to DB
                Exception exception;
                int counter = 0;

                // This has to be retried because user should change ccu object
                do
                {
                    exception = null;
                    CCU ccu = null;
                    try
                    {
                        ccu = CCUs.Singleton.GetObjectForEdit(guidCcu);
                        ccu.Cat12ComboLicence = data;
                        CCUs.Singleton.Update(ccu);
                        CCUs.Singleton.EditEnd(ccu);
                    }
                    catch (Exception ex)
                    {
                        exception = ex;

                        try
                        {
                            if(ccu != null)
                                CCUs.Singleton.EditEnd(ccu);
                        }
                        catch { }
                    }
                    counter++;
                } while (exception != null && counter < 5);
            }
            catch
            { }
        }

        private bool HasLicenceInDB(Guid guidCcu, byte[] data)
        {
            try
            {
                byte[] rawData = QuickCrypto.Decrypt(
                    data,
                    COMBO_LICENCE_KEY,
                    COMBO_LICENCE_SALT);

                uint crcCalc = Crc32.ComputeChecksum(rawData, 0, rawData.Length - 4);
                uint crcRead = BitConverter.ToUInt32(rawData, rawData.Length - 4);

                if (crcCalc != crcRead)
                    return true;

                byte[] temp = new byte[16];
                Array.Copy(rawData, 2, temp, 0, 16);
                Guid guidInData = new Guid(temp);

                if (guidCcu != guidInData)
                    return true;

                return rawData[0] > 0;
            }
            catch
            {
                return true;
            }
        }

        /// <summary>
        /// Get amount of combo licences present in current licence file
        /// </summary>
        public int LicenceCount
        {
            get
            {
                try
                {
                    string localisedName;
                    object licenseValue;

                    if (NCASServer.Singleton.GetLicencePropertyInfo(
                        Globals.RequiredLicenceProperties.Cat12ComboCount.ToString(),
                        out localisedName,
                        out licenseValue))
                    {
                        var data = licenseValue as string;
                        if (data != null)
                        {
                            int value;
                            if (int.TryParse(data, out value))
                                return value;
                        }
                        else
                            return (int) licenseValue;
                    }


#if DEBUG
                    return 1;
#else
                    return 0;
#endif
                }
                catch
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// This function allocate licence and save it in database.
        /// </summary>
        /// <param name="guidCcu"></param>
        /// <returns>return true if licence was allocated and saved in database</returns>
        public bool AllocateLicence(Guid guidCcu)
        {
            lock (_ccuWithLicences)
            {
                if (_ccuWithLicences.Contains(guidCcu))
                    return true;

                if (_ccuWithLicences.Count < LicenceCount)
                {
                    _ccuWithLicences.Add(guidCcu);
                    UpdateLicenceInDB(guidCcu, true);

                    _eventInvoker.Clear();
                    _eventInvoker.Enqueue(FreeLicenceCount);
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Release licence will update database information about licence usage. It should be called frum CCU unconfigure.
        /// </summary>
        /// <param name="guidCcu"></param>
        /// <returns>return true if licence was released</returns>
        public bool ReleaseLicence(Guid guidCcu)
        {
            lock (_ccuWithLicences)
            {
                // Update licence in DB
                UpdateLicenceInDB(guidCcu, false); 
                
                if (!_ccuWithLicences.Contains(guidCcu))
                    return false;

                _ccuWithLicences.Remove(guidCcu);

                _eventInvoker.Clear();
                _eventInvoker.Enqueue(FreeLicenceCount);

                return true;
            }
        }

        public bool HasLicence(Guid guidCcu)
        {
            lock (_ccuWithLicences)
            {
                if (_ccuWithLicences.Contains(guidCcu))
                    return true;

                return false;
            }
        }

        public int FreeLicenceCount
        {
            get
            {
                lock (_ccuWithLicences)
                {
                    return (LicenceCount - _ccuWithLicences.Count);
                }
            }
        }

        private void InvokeEvent(int count)
        {
            try
            {
                CCUConfigurationHandler.Singleton
                    .FreeComboLicenceChanged(count);
            }
            catch
            {
            }
        }
    }
}
