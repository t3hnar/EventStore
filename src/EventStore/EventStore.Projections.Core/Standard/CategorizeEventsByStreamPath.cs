// Copyright (c) 2012, Event Store LLP
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are
// met:
// 
// Redistributions of source code must retain the above copyright notice,
// this list of conditions and the following disclaimer.
// Redistributions in binary form must reproduce the above copyright
// notice, this list of conditions and the following disclaimer in the
// documentation and/or other materials provided with the distribution.
// Neither the name of the Event Store LLP nor the names of its
// contributors may be used to endorse or promote products derived from
// this software without specific prior written permission
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
// THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// 
using System;
using EventStore.Core.Services;
using EventStore.Projections.Core.Services;
using EventStore.Projections.Core.Services.Processing;
using Microsoft.Win32;

namespace EventStore.Projections.Core.Standard
{
    public class CategorizeEventsByStreamPath : IProjectionStateHandler
    {
        private readonly char _separator;
        private readonly string _categoryStreamPrefix;

        public CategorizeEventsByStreamPath(string source, Action<string> logger)
        {
            var trimmedSource = source == null ? null : source.Trim();
            if (trimmedSource == null || trimmedSource.Length != 1)
                throw new InvalidOperationException(
                    "Cannot initialize categorize stream projection handler.  One symbol separator must supplied in the source.");
            _separator = trimmedSource[0];
            if (logger != null)
            {
                logger(
                    string.Format(
                        "Categorize stream projection handler has been initialized with separator: '{0}'", _separator));
            }
            // we will need to declare event types we are interested in
            _categoryStreamPrefix = "$ce-";
        }

        public void ConfigureSourceProcessingStrategy(QuerySourceProcessingStrategyBuilder builder)
        {
            builder.FromAll();
            builder.AllEvents();
            builder.SetIncludeLinks();
        }

        public void Load(string state)
        {
        }

        public void Initialize()
        {
        }

        public string GetStatePartition(
            CheckpointTag position, string streamId, string eventType, string category, Guid eventid, int sequenceNumber,
            string metadata, string data)
        {
            throw new NotImplementedException();
        }

        public bool ProcessEvent(
            string partition, CheckpointTag eventPosition, string category1, ResolvedEvent data,
            out string newState, out EmittedEvent[] emittedEvents)
        {
            emittedEvents = null;
            newState = null;
            if (data.PositionStreamId.StartsWith("$"))
                return false;
            var lastSlashPos = data.PositionStreamId.LastIndexOf(_separator);
            if (lastSlashPos < 0)
                return true; // handled but not interesting to us

            var category = data.PositionStreamId.Substring(0, lastSlashPos);

            string linkTarget;
            if (data.EventType == SystemEventTypes.LinkTo) 
                linkTarget = data.Data;
            else 
                linkTarget = data.EventSequenceNumber + "@" + data.EventStreamId;

            emittedEvents = new[]
                {
                    new EmittedLinkToWithRecategorization(
                        _categoryStreamPrefix + category, Guid.NewGuid(), linkTarget, eventPosition, expectedTag: null,
                        originalStreamId: data.PositionStreamId)
                };

            return true;
        }

        public string TransformStateToResult()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }
    }
}
