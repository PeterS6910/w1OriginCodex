using System;
using System.Collections.Generic;
using System.Linq;
using Contal.Cgp.NCAS.Globals;
using Contal.Cgp.NCAS.RemotingCommon;
using Contal.Cgp.NCAS.Server.Beans;

namespace Contal.Cgp.NCAS.Client
{
    public class APBZConflictDialogModel : APBZConflictDialog.IModel
    {
        private readonly IAntiPassBackZones _antiPassBackZones;
        private readonly AntiPassBackZone _antiPassBackZone;

        private readonly ICollection<CardReader> _conflictingReadersNormalAccess;
        private readonly ICollection<CardReader> _conflictingReadersAccessPermitted;
        private readonly ICollection<CardReader> _conflictingReadersAccessInterrupted;

        private ICollection<CardReader> GetConflictiongCardReaders(ApbzCardReaderEntryExitBy entryExitBy)
        {
            var excludedReadersGuids =
                new HashSet<Guid>(
                    _antiPassBackZones
                        .GetExistingEntryCardReaderGuids(
                            _antiPassBackZone.IdAntiPassBackZone,
                            entryExitBy));

            if (entryExitBy == ApbzCardReaderEntryExitBy.AccessPermitted)
                return
                    new HashSet<CardReader>(
                        _antiPassBackZone.EntryCardReaders
                            .Where(kvPair =>
                                excludedReadersGuids.Contains(kvPair.Key.IdCardReader))
                            .Select(kvPair => kvPair.Key));

            return
                new HashSet<CardReader>(
                    _antiPassBackZone.EntryCardReaders
                        .Where(kvPair =>
                            (kvPair.Value.EntryExitBy == entryExitBy
                             || kvPair.Value.EntryExitBy == ApbzCardReaderEntryExitBy.AccessPermitted)
                            && excludedReadersGuids.Contains(kvPair.Key.IdCardReader))
                        .Select(kvPair => kvPair.Key));
        }

        public APBZConflictDialogModel(
            IAntiPassBackZones antiPassBackZones,
            AntiPassBackZone antiPassBackZone)
        {
            _antiPassBackZones = antiPassBackZones;
            _antiPassBackZone = antiPassBackZone;

            _conflictingReadersNormalAccess =
                GetConflictiongCardReaders(ApbzCardReaderEntryExitBy.NormalAccess);

            _conflictingReadersAccessPermitted =
                GetConflictiongCardReaders(ApbzCardReaderEntryExitBy.AccessPermitted);

            _conflictingReadersAccessInterrupted =
                GetConflictiongCardReaders(ApbzCardReaderEntryExitBy.AccessInterupted);
        }

        private IEnumerable<string> GetAntiPassBackZones(
            IEnumerable<CardReader> conflictingReaders,
            ApbzCardReaderEntryExitBy entryExitBy)
        {
            var apbzModifyObjectsForEntryCardReaders =
                _antiPassBackZones.GetAPBZModifyObjectsForEntryCardReaders(
                    new LinkedList<Guid>(
                        conflictingReaders
                            .Select(cardReader => cardReader.IdCardReader)),
                    entryExitBy);

            return
                apbzModifyObjectsForEntryCardReaders
                    .Where(
                        apbzModifyObject =>
                            apbzModifyObject.GetId != _antiPassBackZone.IdAntiPassBackZone)
                    .Select(apbzModifyObject => apbzModifyObject.ToString());
        }

        public IEnumerable<string> CardReadersNormalAccess
        {
            get
            {
                return
                    _conflictingReadersNormalAccess.Select(
                        modifyObject => modifyObject.ToString());
            }
        }

        public IEnumerable<string> AntiPassBackZonesNormalAccess
        {
            get
            {
                return
                    GetAntiPassBackZones(
                        _conflictingReadersNormalAccess,
                        ApbzCardReaderEntryExitBy.NormalAccess);
            }
        }

        public IEnumerable<string> CardReadersAccessPermitted
        {
            get
            {
                return
                    _conflictingReadersAccessPermitted.Select(
                        modifyObject => modifyObject.ToString());
            }
        }

        public IEnumerable<string> AntiPassBackZonesAccessPermitted
        {
            get
            {
                return
                    GetAntiPassBackZones(
                        _conflictingReadersAccessPermitted,
                        ApbzCardReaderEntryExitBy.AccessPermitted);
            }
        }

        public IEnumerable<string> CardReadersAccessInterrupted
        {
            get
            {
                return
                    _conflictingReadersAccessInterrupted.Select(
                        modifyObject => modifyObject.ToString());
            }
        }

        public IEnumerable<string> AntiPassBackZonesAccessInterrupted
        {
            get
            {
                return
                    GetAntiPassBackZones(
                        _conflictingReadersAccessInterrupted,
                        ApbzCardReaderEntryExitBy.AccessInterupted);
            }
        }

        public APBZConflictDialog.ResultAction ResultAction
        {
            private get;
            set;
        }

        public bool ConflictExists
        {
            get
            {
                return
                    _conflictingReadersNormalAccess.Count != 0
                    || _conflictingReadersAccessPermitted.Count != 0
                    || _conflictingReadersAccessInterrupted.Count != 0;
            }
        }

        public bool PerformAction()
        {
            switch (ResultAction)
            {
                case APBZConflictDialog.ResultAction.ToggleAccessMode:

                    ToggleAccessMode();

                    return true;

                case APBZConflictDialog.ResultAction.RemoveFromThisZone:

                    RemoveFromThisZone(_conflictingReadersNormalAccess);
                    RemoveFromThisZone(_conflictingReadersAccessPermitted);
                    RemoveFromThisZone(_conflictingReadersAccessInterrupted);

                    return true;

                case APBZConflictDialog.ResultAction.RemoveFromOtherZones:

                    RemoveFromOtherZones(
                        _conflictingReadersNormalAccess,
                        ApbzCardReaderEntryExitBy.NormalAccess);


                    RemoveFromOtherZones(
                        _conflictingReadersAccessPermitted,
                        ApbzCardReaderEntryExitBy.AccessPermitted);

                    RemoveFromOtherZones(
                        _conflictingReadersAccessInterrupted,
                        ApbzCardReaderEntryExitBy.AccessInterupted);

                    break;
            }

            return false;
        }

        private void ToggleAccessMode()
        {
            var canNotBeToggled = new LinkedList<CardReader>(_conflictingReadersAccessPermitted);

            var excludedReadersGuids =
                new HashSet<Guid>(_antiPassBackZones
                    .GetExistingEntryCardReaderGuids(
                        _antiPassBackZone.IdAntiPassBackZone,
                        ApbzCardReaderEntryExitBy.NormalAccess));

            var canBeToggledToNormalAccess = new LinkedList<CardReader>();

            foreach (var cardReader in _conflictingReadersAccessInterrupted)
            {
                if (excludedReadersGuids.Contains(cardReader.IdCardReader))
                    canNotBeToggled.AddLast(cardReader);
                else
                    canBeToggledToNormalAccess.AddLast(cardReader);
            }

            ToggleAccessMode(
                canBeToggledToNormalAccess,
                ApbzCardReaderEntryExitBy.NormalAccess);

            excludedReadersGuids =
                new HashSet<Guid>(_antiPassBackZones
                    .GetExistingEntryCardReaderGuids(
                        _antiPassBackZone.IdAntiPassBackZone,
                        ApbzCardReaderEntryExitBy.AccessInterupted));

            var canBeToggledToAccessInterrupted = new LinkedList<CardReader>();

            foreach (var cardReader in _conflictingReadersNormalAccess)
            {
                if (excludedReadersGuids.Contains(cardReader.IdCardReader))
                    canNotBeToggled.AddLast(cardReader);
                else
                    canBeToggledToAccessInterrupted.AddLast(cardReader);
            }

            ToggleAccessMode(
                canBeToggledToAccessInterrupted,
                ApbzCardReaderEntryExitBy.AccessInterupted);

            RemoveFromThisZone(canNotBeToggled);
        }

        private void ToggleAccessMode(
            IEnumerable<CardReader> cardReaders,
            ApbzCardReaderEntryExitBy newEntryExitBy)
        {
            foreach (var conflictingReader in cardReaders)
                _antiPassBackZone.EntryCardReaders[conflictingReader].EntryExitBy = newEntryExitBy;
        }

        private void RemoveFromThisZone(IEnumerable<CardReader> conflictingReaders)
        {
            var entryCardReaders = _antiPassBackZone.EntryCardReaders;

            foreach (var conflictingReader in conflictingReaders)
                entryCardReaders.Remove(conflictingReader);
        }

        private void RemoveFromOtherZones(
            IEnumerable<CardReader> conflictingReaders,
            ApbzCardReaderEntryExitBy entryExitBy)
        {
            _antiPassBackZones.RemoveEntryCardReadersFromAntiPassBackZones(
                _antiPassBackZone.IdAntiPassBackZone,
                new LinkedList<Guid>(
                    conflictingReaders
                        .Select(cardReader => cardReader.IdCardReader)),
                entryExitBy);
        }
    }
}