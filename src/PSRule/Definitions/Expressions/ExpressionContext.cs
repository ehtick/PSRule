// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using PSRule.Pipeline;
using PSRule.Runtime;
using PSRule.Runtime.ObjectPath;

namespace PSRule.Definitions.Expressions
{
    internal interface IExpressionContext : IBindingContext
    {
        string LanguageScope { get; }

        void Reason(IOperand operand, string text, params object[] args);

        void Reason(string text, params object[] args);

        RunspaceContext GetContext();
    }

    internal sealed class ExpressionContext : IExpressionContext, IBindingContext
    {
        private readonly Dictionary<string, PathExpression> _NameTokenCache;

        private List<string> _Reason;

        internal ExpressionContext(SourceFile source)
        {
            Source = source;
            LanguageScope = source.Module;
            _NameTokenCache = new Dictionary<string, PathExpression>();
        }

        public SourceFile Source { get; }

        public string LanguageScope { get; }

        [DebuggerStepThrough]
        void IBindingContext.CachePathExpression(string path, PathExpression expression)
        {
            _NameTokenCache[path] = expression;
        }

        [DebuggerStepThrough]
        bool IBindingContext.GetPathExpression(string path, out PathExpression expression)
        {
            return _NameTokenCache.TryGetValue(path, out expression);
        }

        internal void Debug(string message, params object[] args)
        {
            if (RunspaceContext.CurrentThread?.Writer == null)
                return;

            RunspaceContext.CurrentThread.Writer.WriteDebug(message, args);
        }

        internal void PushScope(RunspaceScope scope)
        {
            RunspaceContext.CurrentThread.PushScope(scope);
            RunspaceContext.CurrentThread.EnterSourceScope(Source);
        }

        internal void PopScope(RunspaceScope scope)
        {
            RunspaceContext.CurrentThread.PopScope(scope);
        }

        public void Reason(IOperand operand, string text, params object[] args)
        {
            if (string.IsNullOrEmpty(text))
                return;

            if (_Reason == null)
                _Reason = new List<string>();

            if (args == null || args.Length == 0)
                _Reason.Add(string.Concat(operand.ToString(), ": ", text));
            else
                _Reason.Add(string.Concat(operand.ToString(), ": ", string.Format(Thread.CurrentThread.CurrentCulture, text, args)));
        }

        public void Reason(string text, params object[] args)
        {
            if (string.IsNullOrEmpty(text))
                return;

            if (_Reason == null)
                _Reason = new List<string>();

            if (args == null || args.Length == 0)
                _Reason.Add(text);
            else
                _Reason.Add(string.Format(Thread.CurrentThread.CurrentCulture, text, args));
        }

        internal string[] GetReasons()
        {
            return _Reason == null || _Reason.Count == 0 ? Array.Empty<string>() : _Reason.ToArray();
        }

        public RunspaceContext GetContext()
        {
            return RunspaceContext.CurrentThread;
        }
    }
}
