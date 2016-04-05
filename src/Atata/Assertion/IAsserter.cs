﻿using System.Collections;

namespace Atata
{
    public interface IAsserter
    {
        void That(bool condition, string message, params object[] args);
        void NotNull(object actual, string message, params object[] args);
        void AreEqual<T>(T expected, T actual, string message, params object[] args);
        void AreNotEqual<T>(T expected, T actual, string message, params object[] args);
        void Contains(string expected, string actual, string message, params object[] args);
        void StartsWith(string expected, string actual, string message, params object[] args);
        void EndsWith(string expected, string actual, string message, params object[] args);
        void IsMatch(string pattern, string actual, string message, params object[] args);
        void IsSubsetOf(IEnumerable subset, IEnumerable superset, string message, params object[] args);
        void HasNoIntersection(IEnumerable collection1, IEnumerable collection2, string message, params object[] args);
    }
}
