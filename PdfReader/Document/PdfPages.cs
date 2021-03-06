﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PdfReader
{
    public class PdfPages : PdfPageInherit
    {
        public PdfPages(PdfDictionary dictionary)
            : base(dictionary.Parent, dictionary.ParseDictionary)
        {
            Children = new List<PdfPageInherit>();
            foreach (PdfObjectReference reference in MandatoryValue<PdfArray>("Kids").Objects)
            {
                PdfDictionary childDictionary = Document.IndirectObjects.MandatoryValue<PdfDictionary>(reference);
                string type = childDictionary.MandatoryValue<PdfName>("Type").Value;

                if (type == "Page")
                    Children.Add(new PdfPage(childDictionary));
                else if (type == "Pages")
                    Children.Add(new PdfPages(childDictionary));
                else
                    throw new ArgumentException($"Unrecognized dictionary type references from page tree '{type}'.");
            }
        }

        public override void Visit(IPdfObjectVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override void FindLeafPages(List<PdfPage> pages)
        {
            foreach (PdfPageInherit child in Children)
                child.FindLeafPages(pages);
        }

        public List<PdfPageInherit> Children { get; private set; }
    }
}
