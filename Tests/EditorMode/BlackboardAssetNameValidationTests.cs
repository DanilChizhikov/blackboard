using System;
using NUnit.Framework;
using UnityEngine;

namespace DTech.Blackboard.Tests.EditorMode
{
	[TestFixture]
	internal sealed class BlackboardAssetNameValidationTests
	{
		private BlackboardAsset _asset;

		[SetUp]
		public void Setup()
		{
			_asset = ScriptableObject.CreateInstance<BlackboardAsset>();
		}

		[TearDown]
		public void TearDown()
		{
			if (_asset != null)
			{
				UnityEngine.Object.DestroyImmediate(_asset);
			}
		}

		[Test]
		public void AddVariable_WithTrimmedName_SavesNormalizedName()
		{
			_asset.AddVariable(" health ", typeof(int));

			Assert.That(_asset.Variables.Count, Is.EqualTo(1));
			Assert.That(_asset.Variables[0].Name, Is.EqualTo("health"));
		}

		[Test]
		public void AddVariable_WithDuplicateNameByPolicy_ThrowsArgumentException()
		{
			_asset.AddVariable("Health", typeof(int));

			ArgumentException exception = Assert.Throws<ArgumentException>(() => _asset.AddVariable(" health ", typeof(int)));
			Assert.That(exception.Message, Does.Contain("already used"));
		}
	}
}
