namespace Gamepangin.UI
{
    public abstract class DataInfoUI<T> : DataInfoBaseUI where T : class
    {
        protected T data;
        
        public void SetData(T newData)
        {
            if (data != null && data == newData)
                OnInfoUpdate();
            else
            {
                data = newData;

                if (data != null)
                {
                    if (CanEnableInfo())
                    {
                        OnInfoUpdate();
                        return;
                    }
                }

                OnInfoDisabled();
            }
        }

        protected abstract bool CanEnableInfo();
        protected abstract void OnInfoUpdate();
        protected abstract void OnInfoDisabled();
    }
}
