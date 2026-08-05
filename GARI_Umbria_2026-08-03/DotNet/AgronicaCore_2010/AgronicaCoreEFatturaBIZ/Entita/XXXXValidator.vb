'Public Class IdFiscaleIVAValidator :  AbstractValidator<IdFiscaleIVA>
'    {
'        Public IdFiscaleIVAValidator()
'        {
'            RuleFor(id => id.IdPaese)
'                .NotEmpty()
'    .SetValidator(New IsValidValidator < IdPaese > ());
'            RuleFor(id => id.IdCodice)
'                .NotEmpty()
'    .Length(1, 28);
'        }
'    }
Public Class IdFiscaleIVAValidator

End Class