using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarvaterWeb.Data
{
    public record IncomeDocumentDetails
    (
        string Summary,
        string SenderNumber
    );

    public record OutcomeDocumentDetails
        (
        string Summary
        );

    public record ContractDetails
        (
        string ContractSubject,
        string Party1Name,
        string Party2Name,
        string Cost,
        string WithNDS,
        string TotalCost
        );

    public record NoteDetails
        (
        string DocumentType,
        string Topic,
        string Content,
        string Adressees    
        );
}
