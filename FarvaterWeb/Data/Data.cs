using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarvaterWeb.Data
{
    public record IncomeDocumentDetails
    (
        string DocumentType,
        string Summary,
        string SenderNumber,
        string Sender
    );

    public record OutcomeDocumentDetails
        (
        string Summary,
        string Resipient,
        string Performer
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
        //string Adressees,
        string LastName,
        string FirstName
        );

    public record OrderDetails
        (
        string DocumentType,
        string OrderName,
        string OrderContent,
        //string Adressees,
        string LastName,
        string FirstName
        //DateTime SigningDate
        );
}
