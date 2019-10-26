namespace vb.Elastic.Fluent.Search.Objects
{
    internal class EsValue  
    {
        private object _value;
        private object _rangeEnd;

        public EsValue(object pValue)
        {
            _value = pValue;
            _rangeEnd = null;
        }
        public EsValue(object pFrom, object pTo)
        {
            _value = pFrom;
            _rangeEnd = pTo;
        }
        public override string ToString()
        {
            return _value.ToString();
        }
        public object From
        {
            get { return _value; }
            set { _value = value; }
        }
        public object To
        {
            get { return _rangeEnd; }
            set { _rangeEnd = value; }
        }
        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }
    }
 }
