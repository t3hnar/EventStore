﻿// Copyright (c) 2012, Event Store LLP
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
using EventStore.Core.Services.Storage.ReaderIndex;
using EventStore.Core.TransactionLog.LogRecords;
using NUnit.Framework;

namespace EventStore.Core.Tests.Services.Storage.CheckCommitStartingAt
{
    [TestFixture]
    public class when_writing_few_prepares_with_same_expected_version_and_not_committing_them :ReadIndexTestScenario
    {
        private PrepareLogRecord _prepare0;
        private PrepareLogRecord _prepare1;
        private PrepareLogRecord _prepare2;

        protected override void WriteTestScenario()
        {
            _prepare0 = WritePrepare("ES", -1);
            _prepare1 = WritePrepare("ES", -1);
            _prepare2 = WritePrepare("ES", -1);
        }

        [Test]
        public void every_prepare_can_be_commited()
        {
            var res = ReadIndex.CheckCommitStartingAt(_prepare0.LogPosition, WriterCheckpoint.ReadNonFlushed());

            Assert.AreEqual(CommitDecision.Ok, res.Decision);
            Assert.AreEqual("ES", res.EventStreamId);
            Assert.AreEqual(-1, res.CurrentVersion);
            Assert.AreEqual(-1, res.StartEventNumber);
            Assert.AreEqual(-1, res.EndEventNumber);

            res = ReadIndex.CheckCommitStartingAt(_prepare1.LogPosition, WriterCheckpoint.ReadNonFlushed());

            Assert.AreEqual(CommitDecision.Ok, res.Decision);
            Assert.AreEqual("ES", res.EventStreamId);
            Assert.AreEqual(-1, res.CurrentVersion);
            Assert.AreEqual(-1, res.StartEventNumber);
            Assert.AreEqual(-1, res.EndEventNumber);

            res = ReadIndex.CheckCommitStartingAt(_prepare2.LogPosition, WriterCheckpoint.ReadNonFlushed());

            Assert.AreEqual(CommitDecision.Ok, res.Decision);
            Assert.AreEqual("ES", res.EventStreamId);
            Assert.AreEqual(-1, res.CurrentVersion);
            Assert.AreEqual(-1, res.StartEventNumber);
            Assert.AreEqual(-1, res.EndEventNumber);
        }
    }
}
