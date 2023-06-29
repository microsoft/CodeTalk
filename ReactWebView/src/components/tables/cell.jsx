import * as React from 'react';
// import './DataTable.css';

export default function Cell({
  height,
  content,
  fixed,
  header,
}) {

  const fixedClass = fixed ? ' Cell-fixed' : '';
  const headerClass = header ? ' Cell-header' : '';
  const style = height ? {height: `${height}px`} : undefined;
  
  const className = (
    `Cell${fixedClass}${headerClass}`
  );  

  const cellMarkup = header ? (
    // Add scope="col" to thead cells
    <th scope="col" className={className} style={style}>
      {content}
    </th>
  ) : (
    fixed ? (
      // Add scope="row" to the first cell of each tbody row
      <th scope="row" className={className} style={style}>
        {content}
      </th>
    ) : (
      <td className={className} style={style}>
        {content}
      </td>
    )
  );

  return (cellMarkup);
}