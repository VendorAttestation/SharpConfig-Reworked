// Copyright (c) 2013-2018 Cemalettin Dervis, MIT License.
// https://github.com/cemdervis/SharpConfig

namespace SharpConfig
{
    // Enumerates the elements of a Setting that represents an array.
    internal sealed class SettingArrayEnumerator
    {
        private readonly string mStringValue;
        private readonly bool mShouldCalcElemString;
        private int mIdxInString;
        private readonly int mLastRBraceIdx;
        private int mPrevElemIdxInString;
        private int mBraceBalance;
        private bool mIsInQuotes;
        private bool mIsDone;

        public SettingArrayEnumerator(string value, bool shouldCalcElemString)
        {
            mStringValue = value;
            mIdxInString = -1;
            mLastRBraceIdx = -1;
            mShouldCalcElemString = shouldCalcElemString;
            IsValid = true;
            mIsDone = false;

            // Initialize starting index and brace balance
            for (int i = 0; i < value.Length; i++)
            {
                char ch = value[i];
                if (ch == '{')
                {
                    mIdxInString = i + 1;
                    mBraceBalance = 1;
                    mPrevElemIdxInString = i + 1;
                    break;
                }
                if (ch != ' ')
                    break;
            }

            // Abort if no valid '{' occurred
            if (mIdxInString < 0)
            {
                IsValid = false;
                mIsDone = true;
                return;
            }

            // Initialize ending index
            for (int i = value.Length - 1; i >= 0; i--)
            {
                char ch = value[i];
                if (ch == '}')
                {
                    mLastRBraceIdx = i;
                    break;
                }
                if (ch != ' ')
                    break;
            }

            // Abort if no valid '}' occurred
            if (mLastRBraceIdx < 0)
            {
                IsValid = false;
                mIsDone = true;
                return;
            }

            // Check if this is an empty array such as "{ }" or "{}"
            if (mIdxInString == mLastRBraceIdx || !IsNonEmptyValue(mStringValue, mIdxInString, mLastRBraceIdx))
            {
                IsValid = true;
                mIsDone = true;
            }
        }


        private void UpdateElementString(int idx)
        {
            Current = mStringValue.Substring(mPrevElemIdxInString, idx - mPrevElemIdxInString).Trim();

            // Trim the quotes, if present, at the beginning and end
            if (Current.StartsWith("\"") && Current.EndsWith("\""))
            {
                Current = Current.Substring(1, Current.Length - 2);
            }
            else
            {
                if (Current.StartsWith("\""))
                {
                    Current = Current.Substring(1);
                }
                if (Current.EndsWith("\""))
                {
                    Current = Current.Substring(0, Current.Length - 1);
                }
            }
        }


        public bool Next()
        {
            if (mIsDone)
                return false;

            while (mIdxInString <= mLastRBraceIdx)
            {
                char ch = mStringValue[mIdxInString];
                if (ch == '{' && !mIsInQuotes)
                {
                    mBraceBalance++;
                }
                else if (ch == '}' && !mIsInQuotes)
                {
                    mBraceBalance--;
                    if (mIdxInString == mLastRBraceIdx)
                    {
                        if (!IsNonEmptyValue(mStringValue, mPrevElemIdxInString, mIdxInString))
                        {
                            IsValid = false;
                        }
                        else if (mShouldCalcElemString)
                        {
                            UpdateElementString(mIdxInString);
                        }
                        mIsDone = true;
                        break;
                    }
                }
                else if (ch == '\"')
                {
                    int nextQuoteIdx = mStringValue.IndexOf('\"', mIdxInString + 1);
                    if (nextQuoteIdx > 0 && mStringValue[nextQuoteIdx - 1] != '\\')
                    {
                        mIdxInString = nextQuoteIdx;
                        mIsInQuotes = false;
                    }
                    else
                    {
                        mIsInQuotes = true;
                    }
                }
                else if (ch == Configuration.ArrayElementSeparator && mBraceBalance == 1 && !mIsInQuotes)
                {
                    if (!IsNonEmptyValue(mStringValue, mPrevElemIdxInString, mIdxInString))
                    {
                        IsValid = false;
                    }
                    else if (mShouldCalcElemString)
                    {
                        UpdateElementString(mIdxInString);
                    }
                    mPrevElemIdxInString = mIdxInString + 1;
                    mIdxInString++;
                    break;
                }

                mIdxInString++;
            }

            if (mIsInQuotes)
                IsValid = false;

            return IsValid;
        }


        private static bool IsNonEmptyValue(string s, int begin, int end)
        {
            for (int i = begin; i < end; i++)
            {
                if (!char.IsWhiteSpace(s[i]))
                {
                    return true;
                }
            }
            return false;
        }

        public string Current { get; private set; }

        public bool IsValid { get; private set; }
    }
}
