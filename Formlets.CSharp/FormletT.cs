using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;

namespace Formlets.CSharp {
    public class Formlet<T> {
        private readonly FSharpFunc<int, Tuple<Tuple<FSharpList<xml_item>, FSharpFunc<FSharpList<Tuple<string, InputValue>>, Tuple<FSharpList<xml_item>, FSharpOption<T>>>>, int>> f;

        public Formlet(FSharpFunc<int, Tuple<Tuple<FSharpList<xml_item>, FSharpFunc<FSharpList<Tuple<string, InputValue>>, Tuple<FSharpList<xml_item>, FSharpOption<T>>>>, int>> f) {
            this.f = f;
        }

        public static implicit operator FSharpFunc<int, Tuple<Tuple<FSharpList<xml_item>, FSharpFunc<FSharpList<Tuple<string, InputValue>>, Tuple<FSharpList<xml_item>, FSharpOption<T>>>>, int>>(Formlet<T> f) {
            return f.f;
        }

        public Formlet<B> Apply<B>(Formlet<Func<T, B>> a) {
            var ff = Formlet.FormletFSharpFunc<T, B>(a);
            var r = new Formlet<B>(FormletModule.ap(ff.f, this.f));
            return r;
        }

        public FormletResult<T> Run(IEnumerable<KeyValuePair<string, string>> env) {
            var ff = FormletModule.run(f);
            var tuples = env.Select(kv => Tuple.Create(kv.Key, InputValue.NewValue(kv.Value)));
            var list = SeqModule.ToList(tuples);
            var r = ff.Invoke(list);
            var xdoc = XmlWriter.render(r.Item1);
            var value = r.Item2;
            return new FormletResult<T>(xdoc, value);
        }

    }
}