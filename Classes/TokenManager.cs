using System.Collections;

namespace GDPR.Common.Classes
{
    public class TokenManager
    {
        private Hashtable tokenHashTable = new Hashtable();
        
        public TokenManager()
        {
        }

        public TokenManager(Hashtable tokens)
        {
            this.tokenHashTable = tokens;
        }

        public TokenManager(string[] keys, string[] values)
        {
            for (int i = 0; i < (keys.GetUpperBound(0) + 1); i++)
                this.tokenHashTable.Add(keys[i], values[i]);

        }

        public void AddToken(string key, string val)
        {
            if (!this.tokenHashTable.Contains(key))
                this.tokenHashTable.Add(key, val);
            else
                this.tokenHashTable[key] = val;
        }

        public string ReplaceTokens(string TextWithTokens)
        {
            if (string.IsNullOrEmpty(TextWithTokens))
                return "";

            IDictionaryEnumerator myEnumerator = this.tokenHashTable.GetEnumerator();
            string _tempText = TextWithTokens;

            while (myEnumerator.MoveNext())
            {
                string val = "";
                if (myEnumerator.Value != null)
                    val = myEnumerator.Value.ToString();

                _tempText = _tempText.Replace(myEnumerator.Key.ToString(), val);
            }

            return (_tempText);
        }
    }
}
