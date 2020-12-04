namespace ActionsBlockSystem
{
    public class ActionLock
    {
        private bool _isLocked;
        private int _currentLocks;

        public ActionLock()
        {
            _isLocked = false;
            _currentLocks = 0;
        }

        public void AddLock()
        {
            _currentLocks++;
            
            if(_currentLocks == 1)
                _isLocked = true;
        }

        public void RemoveLock()
        { 
            _currentLocks--;

            if (_currentLocks == 0)
                _isLocked = false; 
        }

        public bool CanDoAction() => !_isLocked;
    }
}