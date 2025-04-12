using LargeFileSorter.Common;

namespace LargeFileSorter.FileGenerator
{
    interface IRowGenerator
    {
        FileLine GenerateRow();
    }
}
