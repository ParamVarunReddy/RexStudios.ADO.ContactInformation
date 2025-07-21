# Generate a self-signed certificate
$cert = New-SelfSignedCertificate -Subject "CN=aniket, O=corp, C=aniket.com" -DnsName "www.aniket.com" -Type CodeSigning -KeyUsage DigitalSignature -CertStoreLocation Cert:\CurrentUser\My -FriendlyName "ContactInformation"
# Set a password for the private key (optional)
$pw = ConvertTo-SecureString -String "admin12345" -Force -AsPlainText
# Export the certificate as a PFX file
Export-PfxCertificate -Cert $cert -FilePath 'C:\Contact Information\ContactInformation\bin\Debug\certificate.pfx' -Password $pw
# Display a success message
Write-Host "Self-signed certificate exported as certificate.pfx."

_______________________________________________________________________________________________

Execute this Script first


PS C:\Program Files (x86)\Microsoft SDKs\ClickOnce\SignTool> .\signtool.exe sign /f "C:\Contact Information\ContactInformation\bin\Debug\certificate.pfx" /p "admin12345" "C:\Contact Information\ContactInformation\bin\Debug\RexStudios.ADO.ContactInformation.dll"

Create Managed Identity record,
Get Plugin Assembly Id
Update plugin Dll in to Plugin registration tool
Update Plugin Assembly with managed Identity Code, 

