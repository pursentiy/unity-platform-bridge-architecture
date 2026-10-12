using System;
using System.Collections.Generic;
using System.Linq;
using Core.Logging;
using UnityEngine;

namespace Core.Base
{
    public sealed class DisposableCollection : IDisposable
    {
        private readonly HashSet<IDisposable> _disposables = new HashSet<IDisposable>();       
        private bool _isDisposed;
        
        public void Add(IDisposable disposable)
        {
            if (disposable == null)
                return;

            if (_isDisposed)
            {
                DisposeAction(disposable);
                return;
            }

            _disposables.Add(disposable);
        }

        public bool Remove(IDisposable disposable)
        {
            return _disposables.Remove(disposable);
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;
            var snapshot = _disposables.ToArray();
            _disposables.Clear();

            for (var i = snapshot.Length - 1; i >= 0; i--)
                DisposeAction(snapshot[i]);
        }

        private void DisposeAction(IDisposable x)
        {
            try
            {
                x.Dispose();
            }
            catch (Exception e)
            {
                var type = x != null ? x.GetType().Name : string.Empty;
                LoggerService.LogError($"Error when disposing {type}", e);
            }
        }
    }
}