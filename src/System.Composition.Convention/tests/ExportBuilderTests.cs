﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Composition;
using System.Composition.Convention.UnitTests;
using System.Linq;
using System.Composition.Convention;
using System.Reflection;
using Xunit;

namespace System.Composition.Convention
{
    public class ExportBuilderTests
    {
        private interface IFoo { }

        private class FooImpl : IFoo { }


        [Fact]
        public void ExportInterfaceWithTypeOf1()
        {
            var builder = new ConventionBuilder();
            builder.ForType<FooImpl>().Export<IFoo>();

            var exports = builder.GetCustomAttributes(typeof(FooImpl), typeof(FooImpl).GetTypeInfo()).Where<Attribute>(e => e is ExportAttribute).Cast<ExportAttribute>();
            Assert.Equal(1, exports.Count());
            Assert.Equal(exports.First().ContractType, typeof(IFoo));
        }

        [Fact]
        public void ExportInterfaceWithTypeOf2()
        {
            var builder = new ConventionBuilder();
            builder.ForType(typeof(FooImpl)).Export((c) => c.AsContractType(typeof(IFoo)));

            var exports = builder.GetDeclaredAttributes(typeof(FooImpl), typeof(FooImpl).GetTypeInfo()).Where<Attribute>(e => e is ExportAttribute).Cast<ExportAttribute>();
            Assert.Equal(1, exports.Count());
            Assert.Equal(exports.First().ContractType, typeof(IFoo));
        }


        [Fact]
        public void AsContractTypeOfT_SetsContractType()
        {
            var builder = new ConventionBuilder();
            builder.ForTypesDerivedFrom<IFoo>().Export((e) => e.AsContractType<IFoo>());

            ExportAttribute exportAtt = GetExportAttribute(builder);
            Assert.Equal(typeof(IFoo), exportAtt.ContractType);
            Assert.Null(exportAtt.ContractName);
        }

        [Fact]
        public void AsContractType_SetsContractType()
        {
            var builder = new ConventionBuilder();
            builder.ForTypesDerivedFrom<IFoo>().Export((e) => e.AsContractType(typeof(IFoo)));

            ExportAttribute exportAtt = GetExportAttribute(builder);
            Assert.Equal(typeof(IFoo), exportAtt.ContractType);
            Assert.Null(exportAtt.ContractName);
        }

        [Fact]
        public void AsContractName_SetsContractName()
        {
            var builder = new ConventionBuilder();
            builder.ForTypesDerivedFrom<IFoo>().Export((e) => e.AsContractName("hey"));

            ExportAttribute exportAtt = GetExportAttribute(builder);
            Assert.Equal("hey", exportAtt.ContractName);
            Assert.Null(exportAtt.ContractType);
        }

        [Fact]
        public void AsContractName_AndContractType_SetsContractNameAndType()
        {
            var builder = new ConventionBuilder();
            builder.ForTypesDerivedFrom<IFoo>().Export((e) => e.AsContractName("hey").AsContractType(typeof(IFoo)));

            ExportAttribute exportAtt = GetExportAttribute(builder);
            Assert.Equal("hey", exportAtt.ContractName);
            Assert.Equal(typeof(IFoo), exportAtt.ContractType);
        }

        [Fact]
        public void AsContractName_AndContractType_ComputeContractNameFromType()
        {
            var builder = new ConventionBuilder();
            builder.ForTypesDerivedFrom<IFoo>().Export(e => e.AsContractName(t => "Contract:" + t.FullName).AsContractType<IFoo>());

            ExportAttribute exportAtt = GetExportAttribute(builder);
            Assert.Equal("Contract:" + typeof(FooImpl).FullName, exportAtt.ContractName);
            Assert.Equal(typeof(IFoo), exportAtt.ContractType);
        }

        [Fact]
        public void AddMetadata_AddsExportMetadataAttribute()
        {
            var builder = new ConventionBuilder();
            builder.ForTypesDerivedFrom<IFoo>().Export(e => e.AddMetadata("name", "val"));

            ExportMetadataAttribute exportAtt = GetExportMetadataAttribute(builder);
            Assert.Equal("name", exportAtt.Name);
            Assert.Equal("val", exportAtt.Value);
        }

        [Fact]
        public void AddMetadataFuncVal_AddsExportMetadataAttribute()
        {
            var builder = new ConventionBuilder();
            builder.ForTypesDerivedFrom<IFoo>().Export(e => e.AddMetadata("name", t => t.Name));

            ExportMetadataAttribute exportAtt = GetExportMetadataAttribute(builder);
            Assert.Equal("name", exportAtt.Name);
            Assert.Equal(typeof(FooImpl).Name, exportAtt.Value);
        }

        private static ExportAttribute GetExportAttribute(ConventionBuilder builder)
        {
            var list = builder.GetDeclaredAttributes(typeof(FooImpl), typeof(FooImpl).GetTypeInfo());
            Assert.Equal(1, list.Length);
            return list[0] as ExportAttribute;
        }

        private static ExportMetadataAttribute GetExportMetadataAttribute(ConventionBuilder builder)
        {
            var list = builder.GetDeclaredAttributes(typeof(FooImpl), typeof(FooImpl).GetTypeInfo());
            Assert.Equal(2, list.Length);
            return list[1] as ExportMetadataAttribute;
        }
    }
}
