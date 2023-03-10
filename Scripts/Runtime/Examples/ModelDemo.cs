using System.Collections.Generic;
using FullCircleData.Properties;

namespace FullCircleData.Examples
{
    public class ModelDemo : Model
    {
        public Observable<string> message;
        public Observable<List<string>> stringArray;
    }
}