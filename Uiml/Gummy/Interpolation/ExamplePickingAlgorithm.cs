using System;
using System.Collections.Generic;
using System.Text;

using Uiml.Gummy.Domain;

namespace Uiml.Gummy.Interpolation
{
    public class ExamplePickingAlgorithm : InterpolationAlgorithm
    {
        public ExamplePickingAlgorithm(DomainObject domObj)
            : base(domObj)
        {
        }

        public override void Update(System.Drawing.Size size)
        {
            //Update to the new size...
            if (ExampleRepository.Instance.GetDomainObjectExamples(DomainObject.Identifier).ContainsKey(size))
            {
                DomainObject sizeDom = ExampleRepository.Instance.GetDomainObjectExamples(DomainObject.Identifier)[size];
                DomainObject.CopyUIMLFrom(sizeDom);
                DomainObject.Updated();
            }
        }
    }
}
