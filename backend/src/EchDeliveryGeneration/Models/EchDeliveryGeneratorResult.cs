// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Ech0228_1_0;
using EchDeliveryGeneration.Validation;

namespace EchDeliveryGeneration.Models
{
    public class EchDeliveryGeneratorResult
    {
        public EchDeliveryGeneratorResult(Delivery delivery, PostSignatureValidationResult postSignatureValidationResult)
        {
            Delivery = delivery;
            PostSignatureValidationResult = postSignatureValidationResult;
        }

        public Delivery Delivery { get; }

        public PostSignatureValidationResult PostSignatureValidationResult { get; }
    }
}
